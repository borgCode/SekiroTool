using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class EnemyService(IMemoryService memoryService, HookManager hookManager, IReminderService reminderService) : IEnemyService
{
    public void ToggleNoDeath(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoDeath, isEnabled ? 1 : 0);
    }

    public void ToggleNoDamage(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoDamage, isEnabled ? 1 : 0);
    }

    public void ToggleNoHit(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoHit, isEnabled ? 1 : 0);
    }

    public void ToggleNoAttack(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoAttack, isEnabled ? 1 : 0);
    }

    public void ToggleNoMove(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoMove, isEnabled ? 1 : 0);
    }

    public void ToggleDisableAi(bool isEnabled)
    {
        if (isEnabled) reminderService.ChangeIdolIcon();
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.DisableAi, isEnabled ? 1 : 0);
    }

    public void ToggleNoPostureBuildup(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.AllNoPosture, isEnabled ? 1 : 0);
    }
        
    
    public void ToggleTargetingView(bool isEnabled) =>
        memoryService.WriteUInt8(TargetingView.Base, isEnabled ? 1 : 0);

    public void SkipDragonPhaseOne()
    {
        var dragonHandle = Handles.GetDragonHandle(Offsets.Version);
        if (!dragonHandle.HasValue) return;
        
        var bytes = AsmLoader.GetAsmBytes("DragonSkipPhaseOne");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (memoryService.ReadInt64(WorldChrMan.Base), 0x0 + 2),
            (dragonHandle.Value, 0xA + 2),
            (Functions.GetChrInsWithHandle, 0x18 + 2)
        ]);

        memoryService.AllocateAndExecute(bytes);
    }

    public void ToggleDragonActCombo(byte[] actArray, bool isEnabled, bool shouldDoStage1Twice)
    {

        var code = CodeCaveOffsets.Base + CodeCaveOffsets.DragonActCombosCode;
        
        if (isEnabled)
        {
            reminderService.ChangeIdolIcon();
            var stage = CodeCaveOffsets.Base + CodeCaveOffsets.DragonActCombosStage;
            var attackBeforeManipCount = CodeCaveOffsets.Base + CodeCaveOffsets.AttacksBeforeManipCount;
            var doStage1TwiceFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldDoStage1Twice;
            var staggerDurationCmpVal = CodeCaveOffsets.Base + CodeCaveOffsets.StaggerCmpValue;
            memoryService.WriteUInt8(doStage1TwiceFlag, shouldDoStage1Twice ? 1 : 0);
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
                (code.ToInt64() + 0x57, doStage1TwiceFlag.ToInt64(), 7, 0x57 + 2),
                (code.ToInt64() + 0x5E, doStage1TwiceFlag.ToInt64(), 7, 0x5E + 2),
                (code.ToInt64() + 0xEC, staggerDurationCmpVal.ToInt64(), 8, 0xEC + 4),
                (code.ToInt64() + 0x128, hookLoc + 0x6, 5, 0x128 + 1)
            });
            
            int[] patchOffsets = { 0x42, 0x56, 0x107, 0x110 };

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

    public void ToggleSnakeCanyonIntroAnimationLoop(bool isEnabled)
    {
        if (isEnabled)
        {
            RunSnakeCanyonIntroAnimationLoop();
        }
        else
        {
            TerminateSnakeAnimationLoop();
        }
    }

    

    public void RunSnakeCanyonIntroAnimationLoop()
    {
        reminderService.ChangeIdolIcon();
        
        var sleepAddr = memoryService.GetProcAddress("kernel32.dll", "Sleep");
        var code =  CodeCaveOffsets.Base + CodeCaveOffsets.SnakeCanyonIntoAnimationLoopCode;
        var exitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitSnakeLoopFlag;
        var runningFlag = CodeCaveOffsets.Base + CodeCaveOffsets.SnakeLoopIsRunningFlag;
        
        memoryService.WriteUInt8(exitFlag, 0);
        
        var bytes = AsmLoader.GetAsmBytes("SnakeCanyonIntroAnimationLoop");
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code.ToInt64() + 0x0, runningFlag.ToInt64(), 7, 0x0 + 2),
            (code.ToInt64() + 0x9, runningFlag.ToInt64(), 7, 0x9 + 2),
            (code.ToInt64() + 0x1e, exitFlag.ToInt64(), 7, 0x1e + 2),
            (code.ToInt64() + 0x31, Functions.ForceAnimation, 5, 0x31 + 1),
            (code.ToInt64() + 0x48, runningFlag.ToInt64(), 7, 0x48 + 2),
        ]);
        
        Array.Copy(BitConverter.GetBytes(sleepAddr), 0, bytes, 0x14 + 2, 8);
        
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code, 0);
    }

    public void TerminateSnakeAnimationLoop()
    {
        var exitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitSnakeLoopFlag;
        memoryService.WriteUInt8(exitFlag, 1);
    }

    
    
    public void SkipGeni3ByHpWrite()
    {
        memoryService.WriteInt32(CodeCaveOffsets.Base + CodeCaveOffsets.EntityIdInput, 1120830);
        
        var bytes = AsmLoader.GetAsmBytes("SkipGeni3");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (CodeCaveOffsets.Base.ToInt64() + CodeCaveOffsets.EntityIdInput, 0x0 + 2),
            (Functions.GetChrInsByEntityId, 0xe + 2)
        });
        memoryService.AllocateAndExecute(bytes);
    }
    
}