using System.Diagnostics;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EnemyService(IMemoryService memoryService, HookManager hookManager) : IEnemyService
{
    public void ToggleNoDeath(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoDeath, isEnabled ? 1 : 0);

    public void ToggleNoDamage(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoDamage, isEnabled ? 1 : 0);

    public void ToggleNoHit(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoHit, isEnabled ? 1 : 0);

    public void ToggleNoAttack(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoAttack, isEnabled ? 1 : 0);

    public void ToggleNoMove(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoMove, isEnabled ? 1 : 0);

    public void ToggleDisableAi(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.DisableAi, isEnabled ? 1 : 0);

    public void ToggleNoPostureBuildup(bool isEnabled) =>
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.AllNoPosture, isEnabled ? 1 : 0);
    
    public void ToggleTargetingView(bool isEnabled) =>
        memoryService.WriteUInt8(TargetingView.Base, isEnabled ? 1 : 0);

    public void SkipDragonPhaseOne()
    {
        var bytes = AsmLoader.GetAsmBytes("DragonSkipPhaseOne");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (memoryService.ReadInt64(WorldChrMan.Base), 0x0 + 2),
            (Functions.GetEnemyInsWithPackedWorldIdAndChrId, 0x18 + 2)
        ]);

        memoryService.AllocateAndExecute(bytes);
    }

    public void ToggleDragonActCombo(byte[] actArray, bool isEnabled)
    {

        var code = CodeCaveOffsets.Base + CodeCaveOffsets.DragonActCombosCode;
        
        if (isEnabled)
        {
            var stage = CodeCaveOffsets.Base + CodeCaveOffsets.DragonActCombosStage;
            var attackBeforeManipCount = CodeCaveOffsets.Base + CodeCaveOffsets.AttacksBeforeManipCount;
            var staggerDurationCmpVal = CodeCaveOffsets.Base + CodeCaveOffsets.StaggerCmpValue;
            memoryService.WriteUInt8(stage, 0);
            memoryService.WriteUInt8(attackBeforeManipCount, 0);
            memoryService.WriteFloat(staggerDurationCmpVal, 3.9f);
            var hookLoc = Hooks.SetLastAct;
            
            var bytes = AsmLoader.GetAsmBytes("DragonActCombo");
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64() + 0x14, stage.ToInt64(), 7, 0x14 + 3),
                (code.ToInt64() + 0x29, attackBeforeManipCount.ToInt64(), 6, 0x29 + 2),
                (code.ToInt64() + 0x2F, attackBeforeManipCount.ToInt64(), 7, 0x2F + 2),
                (code.ToInt64() + 0xD8, staggerDurationCmpVal.ToInt64(), 8, 0xD8 + 4),
                (code.ToInt64() + 0x114, hookLoc + 0x6, 5, 0x114 + 1)
            });
            
            int[] patchOffsets = { 0x42, 0x56, 0xf3, 0xfc };

            for (int i = 0; i < Math.Min(actArray.Length, patchOffsets.Length); i++)
            {
                bytes[patchOffsets[i]] = actArray[i];
            }
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0x88, 0x98, 0x42, 0xB7, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
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