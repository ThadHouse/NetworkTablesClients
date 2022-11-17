using System.Buffers;
using System.Collections;
using System.Net.WebSockets;
using MessagePack;

public readonly struct ReceiveMessage : IDisposable
{
    public ref struct BinaryMessageEnumerator
    {
        private MessagePackReader reader;

        public BinaryMessageEnumerator(ReadOnlyMemory<byte> data) {
            reader = new MessagePackReader(data);
        }

        public ReadBinaryMessage Current { get; private set; } = default;

        public bool MoveNext()
        {
            if (reader.End) {
                return false;
            }
            MessagePackType next = reader.NextMessagePackType;
            int count = reader.ReadArrayHeader();
            if (count != 4) {
                throw new InvalidDataException("Data is incorrect");
            }
            long topicId = reader.ReadInt64();
            long timestamp = reader.ReadInt64();
            byte type = reader.ReadByte();
            ReadOnlySequence<byte> data = reader.ReadRaw();
            Current = new ReadBinaryMessage(topicId, timestamp, type, data);
            return true;
            // Try to read a value
        }
    }


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

    public BinaryMessageEnumerator GetEnumerator()
    {
        return new BinaryMessageEnumerator(memory);
    }
}
