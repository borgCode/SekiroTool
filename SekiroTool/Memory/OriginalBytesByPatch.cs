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
}