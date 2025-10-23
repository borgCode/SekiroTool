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

    public void ToggleButterflyNoSummons(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ButterflyNoSummons;

        if (isEnabled)
        {
            var hookLoc = Hooks.AiHasSpEffect;
            var bytes = AsmLoader.GetAsmBytes("ButterflyNoSummons");
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x24);
            Array.Copy(jmpBytes, 0, bytes, 0x1F + 1, jmpBytes.Length);
            memoryService.WriteBytes(code, bytes);

            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0xFF, 0x90, 0x08, 0x01, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
       
        
    }
}