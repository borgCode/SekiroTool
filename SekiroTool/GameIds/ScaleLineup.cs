namespace SekiroTool.GameIds;

public class ScaleLineup(int startId, int endId, int unk)
{
    public int StartId { get; } = startId;
    public int EndId { get; } = endId;
    public int Unk { get; } = unk;
    
    
    public static readonly ScaleLineup Harunaga = new ScaleLineup(1000000, 1000099, 1);
    public static readonly ScaleLineup Koremori = new ScaleLineup(2500000, 2500099, 1);
}