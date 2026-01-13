// 

using static SekiroTool.Enums.Patch;

namespace SekiroTool.Memory;

public static class OriginalBytesByPatch
{
    public static class StartMusic
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
            Version1_6_0 => [0x40, 0x57, 0x48, 0x83, 0xEC, 0x30],
            _ => [0x48, 0x89, 0x74, 0x24, 0x10]
        };
    }

    
    public static class ShowSmallHintBox
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
            Version1_2_0 => [0xE8, 0xD8, 0x91, 0x4E, 0x00],
            Version1_3_0 or Version1_4_0 => [0xE8, 0x08, 0x96, 0x4E, 0x00],
            Version1_5_0 => [ 0xE8, 0xC8, 0xBB, 0x50, 0x00],
            Version1_6_0 => [0xE8, 0x38, 0xBF, 0x50, 0x00]
        };
    }
    
    public static class ShowTutorialText
    {
        public static byte[] GetOriginal() => Offsets.Version switch
        {
            Version1_2_0 => [0xE8, 0x88, 0x92, 0x4E, 0x00],
            Version1_3_0 or Version1_4_0  => [0xE8, 0xB8, 0x96, 0x4E, 0x00],
            Version1_5_0 => [0xE8, 0x78, 0xBC, 0x50, 0x00],
            Version1_6_0 => [0xE8, 0xE8, 0xBF, 0x50, 0x00]
           
        };
    }
}