using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class SettingsService(IMemoryService memoryService, NopManager nopManager, HookManager hookManager) : ISettingsService
{
    public void Quitout() =>
        memoryService.WriteUInt8((IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.Quitout, 1);

    public void ToggleNoLogo(bool isEnabled) => memoryService.WriteBytes(Patches.NoLogo, isEnabled ? [0xEB] : [0x74]);
    
    public void ToggleNoTutorials(bool isEnabled)
    {
        if (isEnabled)
        {
            nopManager.InstallNop(Patches.MenuTutorialSkip, 4);
            nopManager.InstallNop(Patches.ShowSmallHintBox, 5);
            nopManager.InstallNop(Patches.ShowTutorialText, 5);
        }
        else
        {
            nopManager.RestoreNop(Patches.MenuTutorialSkip);
            nopManager.RestoreNop(Patches.ShowSmallHintBox);
            nopManager.RestoreNop(Patches.ShowSmallHintBox);
        }
    }

    public void ToggleSaveInCombat(bool isEnabled)
    {
        if (isEnabled) memoryService.WriteBytes(Patches.SaveInCombat, [0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90]);
        else memoryService.WriteBytes(Patches.SaveInCombat, [0x80, 0xB9, 0xFC, 0x11, 0x00, 0x00, 0x03, 0x74, 0x4B]);
    }

    public void ToggleNoCameraSpin(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoCameraSpin;
        if (isEnabled)
        {
            var inputManager = DlUserInputManager.Base;
            var hookLoc = Hooks.GetMouseDelta;
            var bytes = AsmLoader.GetAsmBytes("NoCameraSpin");
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (code.ToInt64() + 0x1, inputManager.ToInt64(), 7, 0x1 + 3),
                (code.ToInt64() + 0x1C, hookLoc + 0x7, 5, 0x1C + 1)
            });
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0x0F, 0x29, 0x83, 0xD0, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void ToggleDisableMusic(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoMenuMusic;
        if (isEnabled)
        {
            StopMusic();

            var hookLoc = Hooks.StartMusic;
            var bytes = AsmLoader.GetAsmBytes("NoMenuMusic");
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x10);
            Array.Copy(jmpBytes, 0, bytes, 0xB + 1, jmpBytes.Length);
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0x40, 0x57, 0x48, 0x83, 0xEC, 0x30]);
        }
        else
        {
           hookManager.UninstallHook(code.ToInt64());
        }
    }

    private void StopMusic()
    {
        var bytes = AsmLoader.GetAsmBytes("StopMusic");
        var funcBytes = BitConverter.GetBytes(Functions.StopMusic);
        Array.Copy(funcBytes, 0, bytes, 0x4 + 2, 8 );
        memoryService.AllocateAndExecute(bytes);
    }

    public void PatchDefaultSound(int defaultSoundVolume)
    {
        var defaultSoundWrite = Patches.DefaultSoundVolWrite;
        memoryService.WriteUInt8(defaultSoundWrite + 0x4, defaultSoundVolume);
        memoryService.WriteUInt8(defaultSoundWrite + 0x5, defaultSoundVolume);
    }
}

