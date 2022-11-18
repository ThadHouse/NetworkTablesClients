using System.Buffers;
using System.Text.Json;
using MessagePack;

public readonly struct ReceiveMessage : IDisposable
{
    public readonly struct BinaryMessageEnumerable {
        private readonly ReadOnlySequence<byte> data;

        public BinaryMessageEnumerable(ReadOnlySequence<byte> data)
        {
            this.data = data;
        }

        public BinaryMessageEnumerator GetEnumerator() {
            return new BinaryMessageEnumerator(data);
        }
    }

    public ref struct BinaryMessageEnumerator
    {
        private MessagePackReader reader;

        public BinaryMessageEnumerator(ReadOnlySequence<byte> data) {
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
        }
    }

    public ReceiveMessageType MessageType {get;}

    private readonly byte[]? memoryOwner;
    private readonly JsonDocument? text;

    private readonly Memory<byte> memory;

    public ReceiveMessage(byte[] memoryOwner, Memory<byte> memory) {
        MessageType = ReceiveMessageType.Binary;
        this.memoryOwner = memoryOwner;
        this.memory = memory;
        this.text = null;
    }

    public ReceiveMessage() {
        MessageType = ReceiveMessageType.Disconnected;
        this.memoryOwner = null;
        this.text = null;
        this.memory = default;
    }

    public ReceiveMessage(JsonDocument document) {
        MessageType = ReceiveMessageType.Text;
        this.memoryOwner = null;
        this.memory = default;
        this.text = document;
    }

    public void Dispose()
    {
        if (memoryOwner != null) {
            ArrayPool<byte>.Shared.Return(memoryOwner);
        }
    }

    public BinaryMessageEnumerable Binary => new BinaryMessageEnumerable(new ReadOnlySequence<byte>(memory));

    public JsonDocument Text => text!;
}
