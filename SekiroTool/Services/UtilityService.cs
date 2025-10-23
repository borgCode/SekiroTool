using System.Collections;
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
        var triggersCode = CodeCaveOffsets.Base + CodeCaveOffsets.TriggersCode;
        var coordUpdateCode = CodeCaveOffsets.Base + CodeCaveOffsets.CoordsUpdate;

        if (isEnabled)
        {
            var worldChrMan = WorldChrMan.Base;
            var inAirTimerHook = Hooks.InAirTimer;
            var bytes = AsmLoader.GetAsmBytes("NoClip_InAirTimer");
            Array.Copy(BitConverter.GetBytes(worldChrMan.ToInt64()), 0, bytes, 0x9 + 2, 8);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(inAirTimerHook, 8, inAirTimerCode + 0x29);
            Array.Copy(jmpBytes, 0, bytes, 0x24 + 1, 4);
            memoryService.WriteBytes(inAirTimerCode, bytes);

            var zDirectionLoc = CodeCaveOffsets.Base + CodeCaveOffsets.ZDirection;
            var keyboardHook = Hooks.KeyBoard;
            bytes = AsmLoader.GetAsmBytes("NoClip_Keyboard");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (keyboardCode.ToInt64() + 0x18, zDirectionLoc.ToInt64(), 7, 0x18 + 2),
                (keyboardCode.ToInt64() + 0x23, zDirectionLoc.ToInt64(), 7, 0x23 + 2),
                (keyboardCode.ToInt64() + 0x2C, keyboardHook + 0x6, 5, 0x2C + 1),
            });
            memoryService.WriteBytes(keyboardCode, bytes);


            bytes = AsmLoader.GetAsmBytes("NoClip_Triggers");
            var triggersHook = Hooks.PadTriggers;
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (triggersCode.ToInt64() + 0x11, triggersHook + 0x5, 5, 0x11 + 1),
                (triggersCode.ToInt64() + 0x16, zDirectionLoc.ToInt64(), 7, 0x16 + 2),
                (triggersCode.ToInt64() + 0x1E, zDirectionLoc.ToInt64(), 7, 0x1E + 2),
            });
            memoryService.WriteBytes(triggersCode, bytes);


            bytes = AsmLoader.GetAsmBytes("NoClip_CoordsUpdate");
            var coordsUpdateHook = Hooks.UpdateCoords;
            var fieldArea = FieldArea.Base;
            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (worldChrMan.ToInt64(), 0x1 + 2),
                (worldChrMan.ToInt64(), 0x26 + 2),
                (fieldArea.ToInt64(), 0x63 + 2)
            });

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (coordUpdateCode.ToInt64() + 0xAF, zDirectionLoc.ToInt64(), 6, 0xAF + 2),
                (coordUpdateCode.ToInt64() + 0xD9, zDirectionLoc.ToInt64(), 7, 0xD9 + 2),
                (coordUpdateCode.ToInt64() + 0xFC, coordsUpdateHook + 0x7, 5, 0xFC + 1)
            });

            memoryService.WriteBytes(coordUpdateCode, bytes);

            hookManager.InstallHook(inAirTimerCode.ToInt64(), inAirTimerHook,
                [0xF3, 0x0F, 0x58, 0x87, 0xD0, 0x08, 0x00, 0x00]);
            hookManager.InstallHook(keyboardCode.ToInt64(), keyboardHook,
                [0xFF, 0x90, 0xF8, 0x00, 0x00, 0x00]);
            hookManager.InstallHook(triggersCode.ToInt64(), triggersHook,
                [0x41, 0x83, 0xF9, 0x0B, 0x74]);
            hookManager.InstallHook(coordUpdateCode.ToInt64(), coordsUpdateHook,
                [0x0F, 0x29, 0xB6, 0x80, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(inAirTimerCode.ToInt64());
            hookManager.UninstallHook(keyboardCode.ToInt64());
            hookManager.UninstallHook(triggersCode.ToInt64());
            hookManager.UninstallHook(coordUpdateCode.ToInt64());
        }
    }
}