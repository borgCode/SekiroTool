using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EnemyService(IMemoryService memoryService, HookManager hookManager) : IEnemyService
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

    
    //TODO Fix, does not work. 
    public void ToggleButterflyNoSnap(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ButterflyNoSnap;

        if (isEnabled)
        {
            var snapAnim = CodeCaveOffsets.Base + CodeCaveOffsets.SnapAnim;
            memoryService.WriteFloat(snapAnim, 3018);
            
            var bytes = AsmLoader.GetAsmBytes("ButterflyNoSnap");
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64() + 0x12, snapAnim.ToInt64(), 7, 0x12 + 3),
                (code.ToInt64() + 0x21, Hooks.AddSubGoal + 0x5, 5, 0x21 + 1)
            });
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), Hooks.AddSubGoal,
                [0x48, 0x89, 0xE0, 0x55, 0x56]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }
}