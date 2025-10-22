namespace SekiroTool.GameIds;

public class ScaleLineup(long startId, long endId, long unk)
{
    public long StartId { get; } = startId;
    public long EndId { get; } = endId;
    public long Unk { get; } = unk;
    
    
    public static readonly ScaleLineup Harunaga = new ScaleLineup(1000000, 1000099, 1);
    public static readonly ScaleLineup Koremori = new ScaleLineup(2500000, 2500099, 1);
}