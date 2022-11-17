using System.Buffers;
using MessagePack;

public readonly ref struct ReadBinaryMessage
{
    public ReadBinaryMessage(long id, long timestamp, byte dataType, ReadOnlySequence<byte> dataBuffer)
    {
        this.Id = id;
        this.Timestamp = timestamp;
        this.DataType = dataType;
        this.DataBuffer = dataBuffer;
    }

    public long Id { get; }
    public long Timestamp { get; }
    public byte DataType { get; }
    public ReadOnlySequence<byte> DataBuffer { get; }

    public bool GetBool() {
        MessagePackReader reader = new(DataBuffer);
        return reader.ReadBoolean();
    }
}
