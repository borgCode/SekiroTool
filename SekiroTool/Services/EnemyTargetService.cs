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

    public ulong GetTargetAddr() =>
        memoryService.ReadUInt64(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget);

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

    public float[] GetPosition()
    {
        var posPtr = memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget,
            [..ChrIns.ChrPhysicsModule, (int)ChrIns.ChrPhysicsOffsets.X]
            ,false);
        
        float[] position = new float[3];
        position[0] = memoryService.ReadFloat(posPtr);
        position[1] = memoryService.ReadFloat(posPtr + 0x4);
        position[2] = memoryService.ReadFloat(posPtr + 0x8);

        return position;
    }

    public void ToggleNoPostureBuildup(bool isEnabled)
    {
        var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.BitFlags;
        memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoPostureConsume, isEnabled);
    }

    public void ToggleNoDeath(bool isEnabled)
    {
        var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.BitFlags;
        memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDeath, isEnabled);
    }

    public void ToggleNoDamage(bool isEnabled)
    {
        var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.BitFlags;
        memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isEnabled);
    }

    public void ToggleFreezePosture(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.FreezeTargetPosture;

        if (isEnabled)
        {
            var hookLoc = Hooks.FreezeTargetPosture;
            var endOfHookedFunc = hookLoc + 0xD1;
            var lockedTargetPtr = CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget;
            var bytes = AsmLoader.GetAsmBytes("FreezeTargetPosture");

            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0x1, lockedTargetPtr.ToInt64(), 7, 0x1 + 3),
                (code.ToInt64() + 0x14, endOfHookedFunc, 6, 0x14 + 2),
                (code.ToInt64() + 0x1F, hookLoc + 0x5, 5, 0x1F + 1)
            ]);

            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc, [0x48, 0x89, 0x5C, 0x24, 0x30]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public int GetLastAct() =>
        memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.LastAct);

    public int GetLastKengekiAct() =>
        memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.LastKengekiAct);

    public void ForceAct(int act) =>
        memoryService.WriteUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct, (byte)act);

    public void ForceKengekiAct(int act) =>
        memoryService.WriteUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceKengekiAct, (byte)act);

    public bool IsTargetRepeating() =>
        memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceAct) != 0;

    public bool IsTargetRepeatingKengeki() =>
        memoryService.ReadUInt8(GetAiThinkPtr() + (int)ChrIns.AiThinkOffsets.ForceKengekiAct) != 0;

    public void ToggleTargetRepeatAct(bool isEnabled)
    {
        var aiThinkPtr = GetAiThinkPtr();
        var forceActPtr = aiThinkPtr + (int)ChrIns.AiThinkOffsets.ForceAct;

        byte value = isEnabled
            ? memoryService.ReadUInt8(aiThinkPtr + (int)ChrIns.AiThinkOffsets.LastAct)
            : (byte)0;

        memoryService.WriteUInt8(forceActPtr, value);
    }

    public void ToggleTargetRepeatKengekiAct(bool isEnabled)
    {
        var aiThinkPtr = GetAiThinkPtr();
        var forceActPtr = aiThinkPtr + (int)ChrIns.AiThinkOffsets.ForceKengekiAct;

        byte value = isEnabled
            ? memoryService.ReadUInt8(aiThinkPtr + (int)ChrIns.AiThinkOffsets.LastKengekiAct)
            : (byte)0;

        memoryService.WriteUInt8(forceActPtr, value);
    }

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget,
            ChrIns.ChrDataModule, true);

    private IntPtr GetAiThinkPtr()
    {
        return memoryService.FollowPointers(CodeCaveOffsets.Base + CodeCaveOffsets.LockedTarget, [
            ChrIns.ComManipulator,
            ChrIns.AiThink
        ], true);
    }

    #endregion
}