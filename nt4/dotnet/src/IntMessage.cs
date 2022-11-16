using MessagePack;

[MessagePackObject]
public readonly struct IntMessage
{
    [Key(0)]
    public int Id { get; }
    [Key(1)]
    public int Timestamp { get; }
    [Key(2)]
    public uint Type { get; } = 0;
    [Key(3)]
    public double LocalTime { get; }

    public bool TrySerialize(Span<byte> memory)
    {
        MessagePackSerializer.Serialize(this);
    }
}
