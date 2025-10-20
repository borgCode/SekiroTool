using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EnemyService(IMemoryService memoryService) : IEnemyService
{
    public void SkipDragonPhaseOne()
    {
        var bytes = AsmLoader.GetAsmBytes("DragonSkipPhaseOne");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (memoryService.ReadInt64(WorldChrMan.Base), 0x0 + 2),
            (Functions.GetEnemyInsWithPackedWorldIdAndChrId, 0x18 + 2)
        ]);
        
        memoryService.AllocateAndExecute(bytes);
    }

    public void ToggleButterflyNoSnap(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ButterflyNoSnap;

        if (isEnabled)
        {
            
        } 
    }
}