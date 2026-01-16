using System.Diagnostics;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class SettingsService(IMemoryService memoryService, NopManager nopManager, HookManager hookManager)
    : ISettingsService
{
    public void Quitout() =>
        memoryService.WriteUInt8((IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.Quitout, 1);

    public async void ToggleNoLogo(bool isEnabled)
    {
        if (isEnabled)
        {
            if (!await WaitForValidBytes(Patches.NoLogo, [0x74]))
                return;
            memoryService.WriteBytes(Patches.NoLogo, [0xEB]);
        }
        else
        {
            memoryService.WriteBytes(Patches.NoLogo, [0x74]);
        }
    }

    public void ToggleNoTutorials(bool isEnabled)
    {
        memoryService.WriteBytes(Patches.MenuTutorialSkip, isEnabled ? [0x90, 0x90, 0x90, 0x90] : [0x84, 0xC0, 0x75, 0x08]);
        memoryService.WriteBytes(Patches.ShowSmallHintBox,
            isEnabled ? [0x90, 0x90, 0x90, 0x90, 0x90] : OriginalBytesByPatch.ShowSmallHintBox.GetOriginal());
        memoryService.WriteBytes(Patches.ShowTutorialText,
            isEnabled ? [0x90, 0x90, 0x90, 0x90, 0x90] : OriginalBytesByPatch.ShowTutorialText.GetOriginal());
    }

    public async void ToggleNoCameraSpin(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoCameraSpin;
        if (isEnabled)
        {
            var inputManager = DlUserInputManager.Base;

            if (!await WaitForValidPtr(inputManager))
                return;

            var hookLoc = Hooks.GetMouseDelta;
            var bytes = AsmLoader.GetAsmBytes("NoCameraSpin");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (code.ToInt64() + 0x1, inputManager.ToInt64(), 7, 0x1 + 3),
                (code.ToInt64() + 0x21, hookLoc + 0x7, 5, 0x21 + 1)
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

    private async Task<bool> WaitForValidPtr(IntPtr ptr, int timeout = 5000)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeout)
        {
            if (memoryService.ReadInt64(ptr) != IntPtr.Zero) return true;
            await Task.Delay(10);
        }

        return false;
    }

    public async void ToggleDisableMusic(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoMenuMusic;
        if (isEnabled)
        {
            var hookLoc = Hooks.StartMusic;
            var originalBytes = OriginalBytesByPatch.StartMusic.GetOriginal();

            if (!await WaitForValidBytes((IntPtr)hookLoc, originalBytes))
                return;


            if (Offsets.Version == Patch.Version1_6_0)
            {
                var bytes = AsmLoader.GetAsmBytes("NoMenuMusic");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x11);
                Array.Copy(jmpBytes, 0, bytes, 0xC + 1, jmpBytes.Length);
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, originalBytes);
            }
            else
            {
                var bytes = AsmLoader.GetAsmBytes("NoMenuMusicEarlyPatches");
                var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(hookLoc, 6, code + 0x10);
                Array.Copy(jmpBytes, 0, bytes, 0xB + 1, jmpBytes.Length);
                memoryService.WriteBytes(code, bytes);
                hookManager.InstallHook(code.ToInt64(), hookLoc, originalBytes);
            }
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    private async Task<bool> WaitForValidBytes(IntPtr address, byte[] expectedBytes, int timeout = 5000)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeout)
        {
            var bytes = memoryService.ReadBytes(address, expectedBytes.Length);
            if (bytes.SequenceEqual(expectedBytes))
                return true;

            await Task.Delay(10);
        }

        return false;
    }

    public void StopMusic()
    {
        var bytes = AsmLoader.GetAsmBytes("StopMusic");
        var funcBytes = BitConverter.GetBytes(Functions.StopMusic);
        Array.Copy(funcBytes, 0, bytes, 0x4 + 2, 8);
        memoryService.AllocateAndExecute(bytes);
    }

    public async void PatchDefaultSound(int defaultSoundVolume)
    {
        var defaultSoundWrite = Patches.DefaultSoundVolWrite;

        byte[] bytes = [0x07, 0x07];
        if (!await WaitForValidBytes(defaultSoundWrite + 0x4, bytes))
            return;

        memoryService.WriteUInt8(defaultSoundWrite + 0x4, defaultSoundVolume);
        memoryService.WriteUInt8(defaultSoundWrite + 0x5, defaultSoundVolume);
    }

    public void ToggleDisableCutscenes(bool isEnabled)
    {
        if (isEnabled)
        {
            nopManager.InstallNop(Functions.FormatCutscenePathString, 146);
        }
        else
        {
            nopManager.RestoreNop(Functions.FormatCutscenePathString);
        }
    }
}