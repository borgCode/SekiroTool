using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class UtilityService(IMemoryService memoryService, HookManager hookManager) : IUtilityService
{
    private const float DefaultNoClipSpeedScaleY = 0.2f;
    private const float DefaultNoClipSpeedScaleX = 0.15f;


    public void ToggleHitboxView(bool isEnabled)
    {
        var hitboxFlagPtr = (IntPtr)memoryService.ReadInt64(DamageManager.Base) + DamageManager.HitboxView;
        memoryService.WriteUInt8(hitboxFlagPtr, isEnabled ? 1 : 0);
    }

    public void TogglePlayerSoundView(bool isEnabled) =>
        memoryService.WriteUInt8(Patches.PlayerSoundView, isEnabled ? 0x75 : 0x74);

    public void ToggleGameRendFlag(int offset, bool isEnabled)
    {
        var flagPtr = GameRendFlags.Base + offset;
        memoryService.WriteUInt8(flagPtr, isEnabled ? 0 : 1);
    }

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

    public void SetGameSpeed(float gameSpeed)
    {
        var gameSpeedPtr = memoryService.ReadInt64(SprjFlipperImp.Base) + SprjFlipperImp.GameSpeed;
        memoryService.WriteFloat((IntPtr)gameSpeedPtr, gameSpeed);
    }

    public float GetGameSpeed()
    {
        var gameSpeedPtr = memoryService.ReadInt64(SprjFlipperImp.Base) + SprjFlipperImp.GameSpeed;
        return memoryService.ReadFloat((IntPtr)gameSpeedPtr);
    }

    public void WriteNoClipSpeed(float speedMultiplier)
    {
        var speedScaleYLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScaleY;
        var speedScaleXLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScaleX;
        memoryService.WriteFloat(speedScaleYLoc, DefaultNoClipSpeedScaleY * speedMultiplier);
        memoryService.WriteFloat(speedScaleXLoc, DefaultNoClipSpeedScaleX * speedMultiplier);
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
            var speedScaleYLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScaleY;
            var speedScaleXLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScaleX;


            AsmHelper.WriteAbsoluteAddresses(bytes, new[]
            {
                (worldChrMan.ToInt64(), 0x1 + 2),
                (worldChrMan.ToInt64(), 0x26 + 2),
                (fieldArea.ToInt64(), 0x62 + 2)
            });

            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (coordUpdateCode.ToInt64() + 0x50, speedScaleYLoc.ToInt64(), 9, 0x50 + 5),
                (coordUpdateCode.ToInt64() + 0x92, speedScaleXLoc.ToInt64(), 9, 0x92 + 5),
                (coordUpdateCode.ToInt64() + 0xAD, zDirectionLoc.ToInt64(), 6, 0xAD + 2),
                (coordUpdateCode.ToInt64() + 0xD7, zDirectionLoc.ToInt64(), 7, 0xD7 + 2),
                (coordUpdateCode.ToInt64() + 0xFA, coordsUpdateHook + 0x7, 5, 0xFA + 1)
            });

            memoryService.WriteBytes(coordUpdateCode, bytes);

            RunRayCast();

            hookManager.InstallHook(inAirTimerCode.ToInt64(), inAirTimerHook,
                [0xF3, 0x0F, 0x58, 0x87, 0xD0, 0x08, 0x00, 0x00]);
            hookManager.InstallHook(keyboardCode.ToInt64(), keyboardHook,
                [0xFF, 0x90, 0xF8, 0x00, 0x00, 0x00]);
            hookManager.InstallHook(triggersCode.ToInt64(), triggersHook,
                [0x4C, 0x8B, 0xDC, 0x53, 0x56]);
            hookManager.InstallHook(coordUpdateCode.ToInt64(), coordsUpdateHook,
                [0x0F, 0x29, 0xB6, 0x80, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(inAirTimerCode.ToInt64());
            hookManager.UninstallHook(keyboardCode.ToInt64());
            hookManager.UninstallHook(triggersCode.ToInt64());
            hookManager.UninstallHook(coordUpdateCode.ToInt64());
            TerminateRayCast();
        }
    }

    private void RunRayCast()
    {
        var havokMan = FrpgHavokMan.Base;
        var worldChrMan = WorldChrMan.Base;
        var fieldArea = FieldArea.Base;
        var sleepAddr = memoryService.GetProcAddress("kernel32.dll", "Sleep");
        var castRay = Functions.FrpgCastRay;

        var rayCastDistanceMultiplier = CodeCaveOffsets.Base + CodeCaveOffsets.RayCastDistanceMultiplier;
        var fromLocation = CodeCaveOffsets.Base + CodeCaveOffsets.From;
        var toLocation = CodeCaveOffsets.Base + CodeCaveOffsets.To;
        var hitEntityLoc = CodeCaveOffsets.Base + CodeCaveOffsets.HitEntity;
        var shouldExitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitFlag;
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.RayCastCode;

        memoryService.WriteFloat(rayCastDistanceMultiplier, 50.0f);
        memoryService.WriteUInt8(shouldExitFlag, 0);
        memoryService.WriteBytes(fromLocation, new byte[16]);
        memoryService.WriteBytes(toLocation, new byte[16]);
        memoryService.WriteBytes(hitEntityLoc, new byte[16]);

        var bytes = AsmLoader.GetAsmBytes("NoClip_RayCast");
        AsmHelper.WriteRelativeOffsets(bytes, new[]
        {
            (code.ToInt64(), shouldExitFlag.ToInt64(), 7, 0x2),
            (code.ToInt64() + 0xD, havokMan.ToInt64(), 7, 0xD + 3),
            (code.ToInt64() + 0x20, worldChrMan.ToInt64(), 7, 0x20 + 3),
            (code.ToInt64() + 0x40, fromLocation.ToInt64(), 7, 0x40 + 3),
            (code.ToInt64() + 0x47, fromLocation.ToInt64() + 0xC, 7, 0x47 + 2),
            (code.ToInt64() + 0x51, fromLocation.ToInt64(), 7, 0x51 + 3),
            (code.ToInt64() + 0x58, fieldArea.ToInt64(), 7, 0x58 + 3),
            (code.ToInt64() + 0x6B, rayCastDistanceMultiplier.ToInt64(), 7, 0x6B + 3),
            (code.ToInt64() + 0x75, toLocation.ToInt64(), 7, 0x75 + 3),
            (code.ToInt64() + 0x7C, toLocation.ToInt64(), 7, 0x7C + 3),
            (code.ToInt64() + 0x99, hitEntityLoc.ToInt64(), 7, 0x99 + 3),
            (code.ToInt64() + 0xA5, castRay, 5, 0xA5 + 1),
            (code.ToInt64() + 0xAE, hitEntityLoc.ToInt64(), 7, 0xAE + 3)
        });

        Array.Copy(BitConverter.GetBytes(sleepAddr), 0, bytes, 0xE8 + 2, 8);
        memoryService.WriteBytes(code, bytes);
        memoryService.RunThread(code, 0);
    }

    private void TerminateRayCast()
    {
        var shouldExitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitFlag;
        memoryService.WriteUInt8(shouldExitFlag, 1);
    }

    public void ToggleFreeCamera(bool isEnabled)
    {
        var camModePtr = memoryService.FollowPointers(FieldArea.Base, FieldArea.FreeCamMode, false);
        memoryService.WriteUInt8(camModePtr, isEnabled ? 1 : 0);
    }

    public void SetCameraMode(int mode) => memoryService.WriteUInt8(PauseRequest.Base, mode);

    public void MoveCamToPlayer()
    {
        byte[] positionBytes = memoryService.ReadBytes(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.X, 12);
        float y = BitConverter.ToSingle(positionBytes, 4);
        y += 5f;
        byte[] modifiedZ = BitConverter.GetBytes(y);
        Buffer.BlockCopy(modifiedZ, 0, positionBytes, 4, 4);
        var freeCamCoordsPtr = memoryService.FollowPointers(FieldArea.Base, FieldArea.DebugCamCoords, false);
        memoryService.WriteBytes(freeCamCoordsPtr, positionBytes);
    }

    public void OpenUpgradePrayerBead()
    {
        var equipInventoryData = memoryService.FollowPointers(GameDataMan.Base, new[]
        {
            GameDataMan.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.EquipInventoryData
        }, true);

        var equipGameData = memoryService.FollowPointers(GameDataMan.Base, new[]
        {
            GameDataMan.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.EquipGameData
        }, true);

        var buttonResult = memoryService.FollowPointers(MenuMan.Base, new[]
        {
            MenuMan.DialogManager,
            MenuMan.GenericDialogButtonResult,
        }, false);

        var mapItemMan = memoryService.ReadInt64(MapItemMan.Base);
        var sleepAddr = memoryService.GetProcAddress("kernel32.dll", "Sleep");

        var menuHandle = CodeCaveOffsets.Base + CodeCaveOffsets.PrayerBeadMenuHandle;


        var bytes = AsmLoader.GetAsmBytes("OpenUpgradePrayerBead");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (Functions.EzStateExternalEventTempCtor, 0x17 + 2),
            (menuHandle.ToInt64(), 0x31 + 2),
            (equipInventoryData.ToInt64(), 0x4A + 2),
            (Functions.GetItemSlot, 0x58 + 2),
            (equipInventoryData.ToInt64(), 0x79 + 2),
            (Functions.GetItemSlot, 0x87 + 2),
            (equipInventoryData.ToInt64(), 0x9C + 2),
            (Functions.GetItemPtrFromSlot, 0xA8 + 2),
            (Functions.SetMessageTagValue, 0xD4 + 2),
            (buttonResult.ToInt64(), 0x14B + 2),
            (Functions.OpenGenericDialog, 0x166 + 2),
            (sleepAddr.ToInt64(), 0x17C + 2),
            (equipInventoryData.ToInt64(), 0x1AF + 2),
            (Functions.GetItemSlot, 0x1BD + 2),
            (equipGameData.ToInt64(), 0x1C9 + 2),
            (Functions.AdjustItemCount, 0x1EF + 2),
            (equipInventoryData.ToInt64(), 0x213 + 2),
            (Functions.GetItemSlot, 0x221 + 2),
            (mapItemMan, 0x247 + 2),
            (Functions.AwardItemLot, 0x259 + 2),
            (Functions.OpenGenericDialog, 0x2DA + 2),
            (Functions.SetMessageTagValue, 0x2F6 + 2),
            (Functions.OpenGenericDialog, 0x377 + 2),
            (Functions.OpenGenericDialog, 0x3FD + 2),
        });

        memoryService.AllocateAndExecute(bytes);
    }

    private IntPtr GetChrPhysicsPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);
}