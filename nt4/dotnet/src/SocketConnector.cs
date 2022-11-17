using System.Buffers;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading.Channels;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

public class SocketConnector : IAsyncDisposable
{
    public const int MaxBufferSize = 2 * 2048 * 2048;

    private async Task ReceiveLoop(ClientWebSocket ws, CancellationToken token)
    {
        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                var rent = MemoryPool<byte>.Shared.Rent(MaxBufferSize);
                var buffer = rent.Memory;
                var totalCount = 0;
                while (true)
                {
                    var result = await ws.ReceiveAsync(buffer, token);
                    totalCount += result.Count;
                    if (result.EndOfMessage)
                    {
                        ReceiveMessage msg;
                        if (result.MessageType is WebSocketMessageType.Close)
                        {
                            msg = new ReceiveMessage(ReceiveMessageType.Disconnected, null, null);
                            readChannel.Writer.Complete();
                            return;
                        }
                        else
                        {
                            var recvMessageType = result.MessageType is WebSocketMessageType.Binary ? ReceiveMessageType.Binary : ReceiveMessageType.Text;
                            msg = new ReceiveMessage(recvMessageType, rent, rent.Memory.Slice(0, totalCount));
                        }
                        await readChannel.Writer.WriteAsync(msg, token);
                        break;
                    }
                }
            }
        }
        catch
        {
            await readChannel.Writer.WriteAsync(new ReceiveMessage(ReceiveMessageType.Disconnected, null, null));
            readChannel.Writer.Complete();
        }
    }

    public ChannelReader<ReceiveMessage> ReadChannel => readChannel.Reader;
    public ChannelWriter<SendMessage> WriteChannel => writeChannel.Writer;

    private Channel<ReceiveMessage> readChannel = Channel.CreateUnbounded<ReceiveMessage>();
    private Channel<SendMessage> writeChannel = Channel.CreateUnbounded<SendMessage>();

    public static async Task<SocketConnector> ConnectAsync(string host, int port, bool secure, string clientName, CancellationToken token = default)
    {
        var conn = new SocketConnector();
        try
        {
            await conn.HandleConnection(host, port, secure, clientName, token);
        }
        catch
        {
            conn.ws.Dispose();
            throw;
        }
        return conn;
    }

    private async Task HandleConnection(string host, int port, bool secure, string clientName, CancellationToken token)
    {
        string type = secure ? "wss" : "ws";
        await ws.ConnectAsync(new Uri($"{type}://{host}:{port}/nt/{clientName}"), token);
        receiveLoop = ReceiveLoop(ws, default); // TODO this needs to be an object token
        writeLoop = WriteLoop(ws); // TODO this needs to be an object token
    }

    private async Task WriteLoop(ClientWebSocket ws)
    {
        try
        {
            while (true)
            {
                using var message = await writeChannel.Reader.ReadAsync();
                await ws.SendAsync(message.Buffer.WrittenMemory, message.MessageType, true, default);
            }
        }
        catch
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        writeChannel.Writer.TryComplete();
        try
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, default);
        }
        catch
        {

        }
        await writeLoop!;
        await receiveLoop!;
        ws.Dispose();
    }

    public async ValueTask Shutdown(WebSocketCloseStatus status, CancellationToken token)
    {
        await ws.CloseOutputAsync(status, null, token);
    }

    private readonly ClientWebSocket ws;
    private Task? receiveLoop;
    private Task? writeLoop;

    private SocketConnector()
    {
        ws = new ClientWebSocket();
        ws.Options.AddSubProtocol("networktables.first.wpi.edu");
        ws.Options.SetBuffer(2 * 1024 * 1024, 2 * 1024 * 1024);
    }
}
