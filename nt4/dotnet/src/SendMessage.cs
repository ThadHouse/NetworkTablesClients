using System.Buffers;
using System.Net.WebSockets;

public readonly struct SendMessage : IDisposable
{
    public WebSocketMessageType MessageType {get;}

    private readonly IMemoryOwner<byte> memoryOwner;
    private readonly Memory<byte> memory;

    public ReadOnlyMemory<byte> Memory => memory;

    public SendMessage(WebSocketMessageType type, IMemoryOwner<byte> memoryOwner, Memory<byte> memory) {
        MessageType = type;
        this.memoryOwner = memoryOwner;
        this.memory = memory;
    }

    public void Dispose()
    {
        memoryOwner.Dispose();
    }
}
