using SekiroTool.Enums;
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
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoDeath, isEnabled);
    }

    public void ToggleNoDamage(bool isEnabled)
    {
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoDamage, isEnabled);
    }

    public void ToggleNoHit(bool isEnabled)
    {
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoHit, isEnabled);
    }

    public void ToggleNoAttack(bool isEnabled)
    {
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoAttack, isEnabled);
    }

    public void ToggleNoMove(bool isEnabled)
    {
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoMove, isEnabled);
    }

    public void ToggleDisableAi(bool isEnabled)
    {
        if (isEnabled) reminderService.ChangeIdolIcon();
        memoryService.Write(DebugFlags.Base + DebugFlags.DisableAi, isEnabled);
    }

    public void ToggleNoPostureBuildup(bool isEnabled)
    {
        memoryService.Write(DebugFlags.Base + DebugFlags.AllNoPosture, isEnabled);
    }
        
    
    public void ToggleTargetingView(bool isEnabled) =>
        memoryService.Write(TargetingView.Base, isEnabled);

    public void SkipDragonPhaseOne()
    {
        var dragonHandle = Handles.GetDragonHandle(Offsets.Version);
        if (!dragonHandle.HasValue) return;
        
        var bytes = AsmLoader.GetAsmBytes(AsmScript.DragonSkipPhaseOne);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (memoryService.Read<nint>(WorldChrMan.Base), 0x0 + 2),
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
            memoryService.Write(doStage1TwiceFlag, shouldDoStage1Twice);
            memoryService.Write(stage, (byte)0);
            memoryService.Write(attackBeforeManipCount, (byte)0);
            memoryService.Write(staggerDurationCmpVal, 3.9f);
            var hookLoc = Hooks.SetLastAct;
            
            var bytes = AsmLoader.GetAsmBytes(AsmScript.DragonActCombo);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code + 0x14, stage, 7, 0x14 + 3),
                (code + 0x29, attackBeforeManipCount, 6, 0x29 + 2),
                (code + 0x2F, attackBeforeManipCount, 7, 0x2F + 2),
                (code + 0x57, doStage1TwiceFlag, 7, 0x57 + 2),
                (code + 0x5E, doStage1TwiceFlag, 7, 0x5E + 2),
                (code + 0xEC, staggerDurationCmpVal, 8, 0xEC + 4),
                (code + 0x128, hookLoc + 0x6, 5, 0x128 + 1)
            ]);
            
            int[] patchOffsets = { 0x42, 0x56, 0x107, 0x110 };

            for (int i = 0; i < Math.Min(actArray.Length, patchOffsets.Length); i++)
            {
                bytes[patchOffsets[i]] = actArray[i];
            }
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code, hookLoc,
                [0x88, 0x98, 0x42, 0xB7, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code);
        }
    }

    public void ToggleButterflyNoSummons(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ButterflyNoSummons;

        if (isEnabled)
        {
            var hookLoc = Hooks.AiHasSpEffect;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.ButterflyNoSummons);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x24);
            Array.Copy(jmpBytes, 0, bytes, 0x1F + 1, jmpBytes.Length);
            memoryService.WriteBytes(code, bytes);

            hookManager.InstallHook(code, hookLoc,
                [0xFF, 0x90, 0x08, 0x01, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code);
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
        
        memoryService.Write(exitFlag, (byte)0);
        
        var bytes = AsmLoader.GetAsmBytes(AsmScript.SnakeCanyonIntroAnimationLoop);
        AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0x0, runningFlag, 7, 0x0 + 2),
            (code + 0x9, runningFlag, 7, 0x9 + 2),
            (code + 0x1e, exitFlag, 7, 0x1e + 2),
            (code + 0x31, Functions.ForceAnimation, 5, 0x31 + 1),
            (code + 0x48, runningFlag, 7, 0x48 + 2),
        ]);
        
        Array.Copy(BitConverter.GetBytes((long)sleepAddr), 0, bytes, 0x14 + 2, 8);
        
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code, 0);
    }

    public void TerminateSnakeAnimationLoop()
    {
        var exitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitSnakeLoopFlag;
        memoryService.Write(exitFlag, (byte)1);
    }
    
}