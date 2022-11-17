using System.Buffers;
using System.Net.WebSockets;
using CommunityToolkit.HighPerformance.Buffers;

public readonly struct SendMessage : IDisposable
{
    public WebSocketMessageType MessageType {get;}

    public ArrayPoolBufferWriter<byte> Buffer {get;}

    public SendMessage(WebSocketMessageType type, ArrayPoolBufferWriter<byte> memoryOwner) {
        MessageType = type;
        this.Buffer = memoryOwner;
    }

    public void Dispose()
    {
        Buffer.Dispose();
    }
}
