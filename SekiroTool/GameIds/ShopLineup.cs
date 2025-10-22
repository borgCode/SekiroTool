namespace SekiroTool.GameIds;

public class ShopLineup(long startId, long endId)
{
    public long StartId { get; } = startId;
    public long EndId { get; } = endId;


    public static readonly ShopLineup Idol = new ShopLineup(1030, 1039);
    public static readonly ShopLineup CrowsBedMemorialMob = new ShopLineup(1100000, 1100049);
    public static readonly ShopLineup BattlefieldMemorialMob = new ShopLineup(1100100, 1100149);
    public static readonly ShopLineup Anayama = new ShopLineup(1100200, 1100249);
    public static readonly ShopLineup Fujioka = new ShopLineup(1100400, 1100449);
    public static readonly ShopLineup DungeonMemorialMob = new ShopLineup(1110000, 1110049);
    public static readonly ShopLineup Badger = new ShopLineup(1111400, 1111449);
    public static readonly ShopLineup ExiledMemorialMob = new ShopLineup(1500000, 1500049);
    public static readonly ShopLineup ToxicMemorialMob = new ShopLineup(1700000, 1700049);
    public static readonly ShopLineup ShugendoMemorialMob = new ShopLineup(2000000, 2000049);
    
    public static readonly ShopLineup Prosthetics = new ShopLineup(1101000, 1101009);
    
}