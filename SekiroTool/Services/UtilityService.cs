using SekiroTool.Enums;
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
        var hitboxFlagPtr = memoryService.Read<nint>(DamageManager.Base) + DamageManager.HitboxView;
        memoryService.Write(hitboxFlagPtr, isEnabled);
    }

    public void TogglePlayerSoundView(bool isEnabled) =>
        memoryService.Write(Patches.PlayerSoundView, isEnabled ? (byte)0x75 : (byte)0x74);

    public void ToggleGameRendFlag(int offset, bool isEnabled)
    {
        var flagPtr = GameRendFlags.Base + offset;
        memoryService.Write(flagPtr, isEnabled);
    }

    public void ToggleMeshFlag(int offset, bool isEnabled)
    {
        memoryService.Write(MeshBase.Base + MeshBase.Mode, (byte)1);
        memoryService.Write(MeshBase.Base + offset, isEnabled);
    }

    public void SetGameSpeed(float gameSpeed)
    {
        nint gameSpeedPtr = memoryService.Read<nint>(SprjFlipperImp.Base) + SprjFlipperImp.GameSpeed;
        memoryService.Write(gameSpeedPtr, gameSpeed);
    }

    public float GetGameSpeed()
    {
        nint gameSpeedPtr = memoryService.Read<nint>(SprjFlipperImp.Base) + SprjFlipperImp.GameSpeed;

        return memoryService.Read<float>(gameSpeedPtr);
    }

    public void WriteNoClipSpeed(float speedMultiplier)
    {
        var speedScaleYLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScale;
        var speedScaleXLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScaleX;
        memoryService.Write(speedScaleYLoc, DefaultNoClipSpeedScaleY * speedMultiplier);
        memoryService.Write(speedScaleXLoc, DefaultNoClipSpeedScaleX * speedMultiplier);
    }

    public void ToggleNoClip(bool isEnabled)
    {
        var inAirTimerCode = CodeCaveOffsets.Base + CodeCaveOffsets.InAirTimer;
        var keyboardCode = CodeCaveOffsets.Base + CodeCaveOffsets.KeyboardCheckCode;
        var triggersCode = CodeCaveOffsets.Base + CodeCaveOffsets.TriggersCode;
        var coordUpdateCode = CodeCaveOffsets.Base + CodeCaveOffsets.CoordsUpdate;
        var physicsPtr = memoryService.FollowPointers(WorldChrMan.Base,
            [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);

        if (isEnabled)
        {
            var worldChrMan = WorldChrMan.Base;
            var inAirTimerHook = Hooks.InAirTimer;
            var bytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_InAirTimer);
            Array.Copy(BitConverter.GetBytes((long)worldChrMan), 0, bytes, 0x9 + 2, 8);
            var jmpBytes = AsmHelper.GetJmpOriginOffsetBytes(inAirTimerHook, 8, inAirTimerCode + 0x29);
            Array.Copy(jmpBytes, 0, bytes, 0x24 + 1, 4);
            memoryService.WriteBytes(inAirTimerCode, bytes);

            var zDirectionLoc = CodeCaveOffsets.Base + CodeCaveOffsets.ZDirection;
            var keyboardHook = Hooks.KeyBoard;
            bytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_Keyboard);
            AsmHelper.WriteRelativeOffsets(bytes, [
                (keyboardCode + 0x18, zDirectionLoc, 7, 0x18 + 2),
                (keyboardCode + 0x23, zDirectionLoc, 7, 0x23 + 2),
                (keyboardCode + 0x2C, keyboardHook + 0x6, 5, 0x2C + 1)
            ]);
            memoryService.WriteBytes(keyboardCode, bytes);


            bytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_Triggers);
            var triggersHook = Hooks.PadTriggers;
            AsmHelper.WriteRelativeOffsets(bytes, [
                (triggersCode + 0x11, triggersHook + 0x5, 5, 0x11 + 1),
                (triggersCode + 0x16, zDirectionLoc, 7, 0x16 + 2),
                (triggersCode + 0x1E, zDirectionLoc, 7, 0x1E + 2)
            ]);
            memoryService.WriteBytes(triggersCode, bytes);


            bytes = AsmLoader.GetAsmBytes(AsmScript.NoClip_CoordsUpdate);
            var coordsUpdateHook = Hooks.UpdateCoords;
            var fieldArea = FieldArea.Base;
            var fd4PadMan = Fd4PadManager.Base;
            var speedScaleLoc = CodeCaveOffsets.Base + CodeCaveOffsets.SpeedScale;

            AsmHelper.WriteRelativeOffsets(bytes, [
                (coordUpdateCode + 0x1, worldChrMan, 7, 0x1 + 3),
                (coordUpdateCode + 0x5C, fd4PadMan, 7, 0x5C + 3),
                (coordUpdateCode + 0x70, Functions.GetRawY, 5, 0x70 + 1),
                (coordUpdateCode + 0x7D, Functions.GetRawX, 5, 0x7D + 1),
                (coordUpdateCode + 0xB1, fieldArea, 7, 0xB1 + 3),
                (coordUpdateCode + 0xC4, Functions.MatrixVectorToProduct, 5, 0xC4 + 1),
                (coordUpdateCode + 0xF9, speedScaleLoc, 9, 0xF9 + 5),
                (coordUpdateCode + 0x116, zDirectionLoc, 6, 0x116 + 2),
                (coordUpdateCode + 0x140, zDirectionLoc, 7, 0x140 + 2),
                (coordUpdateCode + 0x173, coordsUpdateHook + 0x7, 5, 0x173 + 1)
            ]);

            memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, (byte)1);

            memoryService.WriteBytes(coordUpdateCode, bytes);

            // RunRayCast();

            hookManager.InstallHook(inAirTimerCode, inAirTimerHook,
                [0xF3, 0x0F, 0x58, 0x87, 0xD0, 0x08, 0x00, 0x00]);
            hookManager.InstallHook(keyboardCode, keyboardHook,
                [0xFF, 0x90, 0xF8, 0x00, 0x00, 0x00]);
            hookManager.InstallHook(triggersCode, triggersHook,
                [0x4C, 0x8B, 0xDC, 0x53, 0x56]);
            hookManager.InstallHook(coordUpdateCode, coordsUpdateHook,
                [0x0F, 0x29, 0xB6, 0x80, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(inAirTimerCode);
            hookManager.UninstallHook(keyboardCode);
            hookManager.UninstallHook(triggersCode);
            hookManager.UninstallHook(coordUpdateCode);
            memoryService.Write(physicsPtr + (int)ChrIns.ChrPhysicsOffsets.NoGravity, (byte)0);
            // TerminateRayCast();
        }
    }
    //
    // private void RunRayCast()
    // {
    //     var havokMan = FrpgHavokMan.Base;
    //     var worldChrMan = WorldChrMan.Base;
    //     var fieldArea = FieldArea.Base;
    //     var sleepAddr = memoryService.GetProcAddress("kernel32.dll", "Sleep");
    //     var castRay = Functions.FrpgCastRay;
    //
    //     var rayCastDistanceMultiplier = CodeCaveOffsets.Base + CodeCaveOffsets.RayCastDistanceMultiplier;
    //     var fromLocation = CodeCaveOffsets.Base + CodeCaveOffsets.From;
    //     var toLocation = CodeCaveOffsets.Base + CodeCaveOffsets.To;
    //     var hitEntityLoc = CodeCaveOffsets.Base + CodeCaveOffsets.HitEntity;
    //     var shouldExitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitFlag;
    //     var code = CodeCaveOffsets.Base + CodeCaveOffsets.RayCastCode;
    //
    //     memoryService.WriteFloat(rayCastDistanceMultiplier, 50.0f);
    //     memoryService.WriteUInt8(shouldExitFlag, 0);
    //     memoryService.WriteBytes(fromLocation, new byte[16]);
    //     memoryService.WriteBytes(toLocation, new byte[16]);
    //     memoryService.WriteBytes(hitEntityLoc, new byte[16]);
    //
    //     var bytes = AsmLoader.GetAsmBytes("NoClip_RayCast");
    //     AsmHelper.WriteRelativeOffsets(bytes, new[]
    //     {
    //         (code.ToInt64(), shouldExitFlag.ToInt64(), 7, 0x2),
    //         (code.ToInt64() + 0xD, havokMan.ToInt64(), 7, 0xD + 3),
    //         (code.ToInt64() + 0x20, worldChrMan.ToInt64(), 7, 0x20 + 3),
    //         (code.ToInt64() + 0x40, fromLocation.ToInt64(), 7, 0x40 + 3),
    //         (code.ToInt64() + 0x47, fromLocation.ToInt64() + 0xC, 7, 0x47 + 2),
    //         (code.ToInt64() + 0x51, fromLocation.ToInt64(), 7, 0x51 + 3),
    //         (code.ToInt64() + 0x58, fieldArea.ToInt64(), 7, 0x58 + 3),
    //         (code.ToInt64() + 0x6B, rayCastDistanceMultiplier.ToInt64(), 7, 0x6B + 3),
    //         (code.ToInt64() + 0x75, toLocation.ToInt64(), 7, 0x75 + 3),
    //         (code.ToInt64() + 0x7C, toLocation.ToInt64(), 7, 0x7C + 3),
    //         (code.ToInt64() + 0x99, hitEntityLoc.ToInt64(), 7, 0x99 + 3),
    //         (code.ToInt64() + 0xA5, castRay, 5, 0xA5 + 1),
    //         (code.ToInt64() + 0xAE, hitEntityLoc.ToInt64(), 7, 0xAE + 3)
    //     });
    //
    //     Array.Copy(BitConverter.GetBytes(sleepAddr), 0, bytes, 0xE8 + 2, 8);
    //     memoryService.WriteBytes(code, bytes);
    //     memoryService.RunThread(code, 0);
    // }
    //
    // private void TerminateRayCast()
    // {
    //     var shouldExitFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ShouldExitFlag;
    //     memoryService.WriteUInt8(shouldExitFlag, 1);
    // }

    public void ToggleFreeCamera(bool isEnabled)
    {
        var camModePtr = memoryService.FollowPointers(FieldArea.Base, FieldArea.FreeCamMode, false);
        memoryService.Write(camModePtr, isEnabled);
    }

    public void SetCameraMode(int mode) => memoryService.Write(PauseRequest.Base, (byte)mode);

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

    public void ToggleSaveInCombat(bool isEnabled)
    {
        if (isEnabled)
            memoryService.WriteBytes(Patches.SaveInCombat, [0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90]);
        else memoryService.WriteBytes(Patches.SaveInCombat, [0x80, 0xB9, 0xFC, 0x11, 0x00, 0x00, 0x03, 0x74, 0x4B]);
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

        var mapItemMan = memoryService.Read<nint>(MapItemMan.Base);
        var sleepAddr = memoryService.GetProcAddress("kernel32.dll", "Sleep");

        var menuHandle = CodeCaveOffsets.Base + CodeCaveOffsets.PrayerBeadMenuHandle;


        var bytes = AsmLoader.GetAsmBytes(AsmScript.OpenUpgradePrayerBead);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (Functions.EzStateExternalEventTempCtor, 0x17 + 2),
            (menuHandle, 0x31 + 2),
            (equipInventoryData, 0x4A + 2),
            (Functions.GetItemSlot, 0x58 + 2),
            (equipInventoryData, 0x79 + 2),
            (Functions.GetItemSlot, 0x87 + 2),
            (equipInventoryData, 0x9C + 2),
            (Functions.GetItemPtrFromSlot, 0xA8 + 2),
            (Functions.SetMessageTagValue, 0xD4 + 2),
            (buttonResult, 0x14B + 2),
            (Functions.OpenGenericDialog, 0x166 + 2),
            (sleepAddr, 0x17C + 2),
            (equipInventoryData, 0x1AF + 2),
            (Functions.GetItemSlot, 0x1BD + 2),
            (equipGameData, 0x1C9 + 2),
            (Functions.AdjustItemCount, 0x1EF + 2),
            (equipInventoryData, 0x213 + 2),
            (Functions.GetItemSlot, 0x221 + 2),
            (mapItemMan, 0x247 + 2),
            (Functions.AwardItemLot, 0x259 + 2),
            (Functions.OpenGenericDialog, 0x2DA + 2),
            (Functions.SetMessageTagValue, 0x2F6 + 2),
            (Functions.OpenGenericDialog, 0x377 + 2),
            (Functions.OpenGenericDialog, 0x3FD + 2),
        ]);

        memoryService.AllocateAndExecute(bytes);
    }

    private IntPtr GetChrPhysicsPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);
}