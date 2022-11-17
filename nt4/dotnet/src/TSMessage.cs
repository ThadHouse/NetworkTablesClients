using MessagePack;

[MessagePackObject]
public class TSMessage {
    [Key(0)]
    public int Id = -1;
    [Key(1)]
    public int Timestamp = 0;
    [Key(2)]
    public int Type = 1;
    [Key(3)]
    public double Current = 42;
}
