using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;

namespace SekiroTool.Services;

public class EnemyTargetService(IMemoryService memoryService, HookManager hookManager) : IEnemyTargetService
{
    public void ToggleTargetHook(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.SaveLockedTargetCode;

        if (isEnabled)
        {
            var hookLoc = Offsets.Hooks.LockedTarget;
            var ptrLoc = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget;
            var bytes = AsmLoader.GetAsmBytes("LockedTarget");

            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64(), ptrLoc.ToInt64(), 7, 0x0 + 3),
                (code.ToInt64() + 0xE, hookLoc + 0x7, 5, 0xE + 1)
            ]);

            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x8B, 0x80, 0xF8, 0x1F, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }
}