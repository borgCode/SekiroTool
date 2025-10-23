using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class UtilityService(IMemoryService memoryService, HookManager hookManager) : IUtilityService
{
    public void OpenSkillMenu()
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuNoParams");
        AsmHelper.WriteAbsoluteAddress(bytes, Functions.OpenSkillMenu, 0x8 + 2);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenUpgradeProstheticsMenu()
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuNoParams");
        AsmHelper.WriteAbsoluteAddress(bytes, Functions.UpgradeProstheticsMenu, 0x8 + 2);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenRegularShop(ShopLineup shopLineup)
    {
        var patchLoc = Patches.OpenRegularShopPatch;
        var originalBytes = memoryService.ReadBytes(patchLoc, 5);
        memoryService.WriteBytes(patchLoc, [0xEB, 0x20, 0x90, 0x90, 0x90]);

        var bytes = AsmLoader.GetAsmBytes("OpenMenuTwoParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (shopLineup.StartId, 0x8 + 2),
            (shopLineup.EndId, 0x12 + 2),
            (Functions.OpenRegularShop, 0x1C + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);

        memoryService.WriteBytes(patchLoc, originalBytes);
    }

    public void OpenScalesShop(ScaleLineup scaleLineup)
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuThreeParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (scaleLineup.StartId, 0x8 + 2),
            (scaleLineup.EndId, 0x12 + 2),
            (scaleLineup.Unk, 0x1C + 2),
            (Functions.OpenScalesShop, 0x26 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenProstheticsShop(ShopLineup shopLineup)
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuTwoParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (shopLineup.StartId, 0x8 + 2),
            (shopLineup.EndId, 0x12 + 2),
            (Functions.OpenProstheticsShop, 0x1C + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void ToggleNoClip(bool isEnabled)
    {
        var inAirTimerCode = CodeCaveOffsets.Base + CodeCaveOffsets.InAirTimer;
        var keyboardCode = CodeCaveOffsets.Base + CodeCaveOffsets.KeyboardCheckCode;
        
        if (isEnabled)
        {
            var worldChrMan = WorldChrMan.Base;
            var inAirTimerHook = Hooks.InAirTimer;
            var bytes = AsmLoader.GetAsmBytes("NoClip_InAirTimer");
            AsmHelper.WriteRelativeOffsets(bytes, [
                (inAirTimerCode.ToInt64() + 0x9, worldChrMan.ToInt64(), 7, 0x9 + 3),
                (inAirTimerCode.ToInt64() + 0x21, inAirTimerHook + 0x8, 5, 0x21 + 1)
            ]);
            memoryService.WriteBytes(inAirTimerCode, bytes);
            
            var zDirectionLoc = CodeCaveOffsets.Base + CodeCaveOffsets.ZDirection;
            var keyboardHook = Hooks.KeyBoard;
            bytes = AsmLoader.GetAsmBytes("NoClip_Keyboard");
            AsmHelper.WriteRelativeOffsets(bytes, new []
            {
                (keyboardCode.ToInt64() + 0x18, zDirectionLoc.ToInt64(), 7, 0x18 + 2),
                (keyboardCode.ToInt64() + 0x23, zDirectionLoc.ToInt64(), 7, 0x23 + 2),
                (keyboardCode.ToInt64() + 0x2C, keyboardHook + 0x6, 5, 0x2C + 1),
            });
            memoryService.WriteBytes(keyboardCode, bytes);
            
            
            
            
            hookManager.InstallHook(inAirTimerCode.ToInt64(), inAirTimerHook,
                [0xF3, 0x0F, 0x58, 0x86, 0x00, 0x09, 0x00, 0x00]);
            hookManager.InstallHook(keyboardCode.ToInt64(), keyboardHook,
                [0xFF, 0x90, 0xF8, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(inAirTimerCode.ToInt64());
            hookManager.UninstallHook(keyboardCode.ToInt64());
        }
    }
}