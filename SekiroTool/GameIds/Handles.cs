using SekiroTool.Enums;

namespace SekiroTool.GameIds;

public static class Handles
{
    private static readonly Dictionary<Patch, int> DragonPatchHandles = new()
    {
        [Patch.V102] = 0x100300b8,
        [Patch.V104] = 0x100300b8,
        [Patch.V105] = 0x100300ba, 
        [Patch.V106] = 0x100300ba
    };
    
    public static int? GetDragonHandle(Patch patch)
    {
        return DragonPatchHandles.TryGetValue(patch, out var handle) ? handle : null;
    }
    
}