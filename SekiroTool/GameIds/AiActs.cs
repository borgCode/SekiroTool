namespace SekiroTool.GameIds;

public static class AiActs
{

    public static class Dragon
    {
        public enum DragonActs
        {
            KanjiSwipe = 9,
            VerticalSlams = 3,
            SlowNeckSwim = 14
        }
        
        public static readonly byte[] Combo1 = 
        [
            (byte)DragonActs.KanjiSwipe,
            (byte)DragonActs.VerticalSlams,
            (byte)DragonActs.VerticalSlams,
            (byte)DragonActs.SlowNeckSwim
        ];
        
        public static readonly byte[] Combo2 = 
        [
            (byte)DragonActs.VerticalSlams,
            (byte)DragonActs.SlowNeckSwim
        ];
        
        public static readonly byte[] Combo3 = 
        [
            (byte)DragonActs.KanjiSwipe,
            (byte)DragonActs.SlowNeckSwim,
            (byte)DragonActs.SlowNeckSwim,
            (byte)DragonActs.VerticalSlams
        ];
    }
    
    
}