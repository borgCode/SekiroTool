using SekiroTool.Enums;

namespace SekiroTool.GameIds;

public static class Handles
{
    private static readonly Dictionary<Patch, int> DragonPatchHandles = new()
    {
        [Patch.Version1_2_0] = 0x100300b8,
        [Patch.Version1_3_0] = 0x100300b8,
        [Patch.Version1_4_0] = 0x100300b8,
        [Patch.Version1_5_0] = 0x100300ba, 
        [Patch.Version1_6_0] = 0x100300ba
    };
    
    public static int? GetDragonHandle(Patch patch)
    {
        return DragonPatchHandles.TryGetValue(patch, out var handle) ? handle : null;
    }
    
}