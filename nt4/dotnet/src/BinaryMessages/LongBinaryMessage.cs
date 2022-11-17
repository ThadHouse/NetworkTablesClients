using MessagePack;

[MessagePackObject]
public readonly struct LongBinaryMessage {
    public LongBinaryMessage(long id, long timestamp, long value)
    {
        Id = id;
        Timestamp = timestamp;
        Value = value;
    }

    [Key(0)]
    public long Id {get;}
    [Key(1)]
    public long Timestamp {get;}
    [Key(2)]
    public byte Type {get;} = 2;
    [Key(3)]
    public long Value {get;}
}
