using MessagePack;

[MessagePackObject]
public class BlockData
{
    [Key(0)]
    public int BlockState { get; set; }

    [Key(1)]
    public int X { get; set; }

    [Key(2)]
    public int Y { get; set; }

    public BlockData() { }

    public BlockData(int blockState, int x, int y)
    {
        BlockState = blockState;
        X = x;
        Y = y;
    }
}