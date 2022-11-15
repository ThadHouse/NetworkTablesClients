using System.Buffers;
using System.Net.WebSockets;
using System.Threading.Channels;

public class SocketConnector
{
    public const int MaxBufferSize = 2 * 2048 * 2048;

    private async Task ReceiveLoop(ClientWebSocket ws, CancellationToken token) {
        try {
        while (true) {
            token.ThrowIfCancellationRequested();
            var rent = MemoryPool<byte>.Shared.Rent(MaxBufferSize);
            var buffer = rent.Memory;
            var totalCount = 0;
            while (true) {
                var result =  await ws.ReceiveAsync(buffer, token);
                totalCount += result.Count;
                if (result.EndOfMessage) {
                    ReceiveMessage msg;
                    if (result.MessageType is WebSocketMessageType.Close) {
                        msg = new ReceiveMessage(ReceiveMessageType.Disconnected, null, null);
                    } else {
                        var recvMessageType = result.MessageType is WebSocketMessageType.Binary ? ReceiveMessageType.Binary : ReceiveMessageType.Text;
                        msg = new ReceiveMessage(recvMessageType, rent, rent.Memory.Slice(0, totalCount));
                    }
                    await readChannel.Writer.WriteAsync(msg, token);
                    break;
                }
            }
        }
        } catch (Exception e) {
            readChannel.Writer.Complete(e);
            throw;
        }
    }

    public ChannelReader<ReceiveMessage> ReadChannel => readChannel.Reader;

    private Channel<ReceiveMessage> readChannel = Channel.CreateUnbounded<ReceiveMessage>();

    public static async Task<SocketConnector> ConnectAsync(string host, int port, bool secure, string clientName, CancellationToken token = default) {
        var conn = new SocketConnector();
        await conn.HandleConnection(host, port, secure, clientName, token);
        return conn;
    }

    private async Task HandleConnection(string host, int port, bool secure, string clientName, CancellationToken token) {
        string type = secure ? "wss" : "ws";
        await ws.ConnectAsync(new Uri($"{type}://{host}:{port}/nt/clientName"), token);
        receiveLoop = ReceiveLoop(ws, default); // TODO this needs to be an object token
    }

    private async Task WriteLoop(ClientWebSocket ws, CancellationToken token) {
        while (true) {

        }
    }

    private readonly ClientWebSocket ws;
    private Task? receiveLoop;

    public SocketConnector() {
        ws = new ClientWebSocket();
        ws.Options.AddSubProtocol("networktables.first.wpi.edu");
        ws.Options.SetBuffer(2 * 1024 * 1024, 2 * 1024 * 1024);
    }


}
