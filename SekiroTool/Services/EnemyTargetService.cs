using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EnemyTargetService(IMemoryService memoryService, HookManager hookManager) : IEnemyTargetService
{
    #region Public Methods

    public void ToggleTargetHook(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.SaveLockedTargetCode;

        if (isEnabled)
        {
            var hookLoc = Hooks.LockedTarget;
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

    public void SetHp(int hp) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Hp, hp);

    public int GetCurrentHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Hp);

    public int GetMaxHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHp);

    public void SetPosture(int posture) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Posture, posture);

    public int GetCurrentPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Posture);

    public int GetMaxPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxPosture);

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget,
            ChrIns.ChrDataModule, true);

    #endregion
}