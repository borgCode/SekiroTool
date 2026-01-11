// 

namespace SekiroTool.GameIds;

public static class EzState
{
    public class TalkCommand(int commandId, int[] @params)
    {
        public int CommandId { get; } = commandId;
        public int[] Params { get; } = @params;
    }

    public static class TalkCommands
    {
        public static readonly TalkCommand OpenUpgrade = new(24, [1]);
        public static readonly TalkCommand LevelUp = new(31, []);

        public static TalkCommand OpenRegularShop(ShopLineup shopLineup) =>
            new(22, [shopLineup.StartId, shopLineup.EndId]);
        
        public static TalkCommand OpenProstheticsShop(ShopLineup shopLineup) =>
            new(111, [shopLineup.StartId, shopLineup.EndId]);
        
        public static TalkCommand OpenScaleShop(ScaleLineup scaleLineup) =>
            new(122, [scaleLineup.StartId, scaleLineup.EndId, scaleLineup.Unk]);

        public static readonly TalkCommand OpenProstheticsUpgrade = new(123, []);
        public static readonly TalkCommand OpenSkillMenu = new(124, []);
    }
}