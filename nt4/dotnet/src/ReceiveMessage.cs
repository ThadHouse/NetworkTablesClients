using System.Buffers;
using System.Net.WebSockets;

public readonly struct ReceiveMessage : IDisposable
{
    public ReceiveMessageType MessageType {get;}

    private readonly IMemoryOwner<byte>? memoryOwner;

    private readonly Memory<byte> memory;

    public ReceiveMessage(ReceiveMessageType type, IMemoryOwner<byte>? memoryOwner, Memory<byte> memory) {
        MessageType = type;
        this.memoryOwner = memoryOwner;
        this.memory = memory;
    }

    public void Dispose()
    {
        memoryOwner?.Dispose();
    }
}
