using System.Runtime.InteropServices;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Models;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class PlayerService(IMemoryService memoryService, HookManager hookManager) : IPlayerService
{
    private Position _position1 = new();
    private Position _position2 = new();
    private Dictionary<int, int> _idolsByAreaIndex = DataLoader.GetIdolIdsByAreaIndexDictionary();

    #region Public Methods

    public void SavePos(int index)
    {
        var chrPhysicsPtr = GetChrPhysicsPtr();

        var areaIndex = memoryService.ReadInt32((IntPtr)memoryService.ReadInt64(FieldArea.Base) +
                                                FieldArea.CurrentWorldBlockIndex);

        byte[] positionBytes = memoryService.ReadBytes(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.X, 16);
        float angle = memoryService.ReadFloat(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.Angle);

        if (index == 0)
        {
            _position1.Xyz = positionBytes;
            _position1.Angle = angle;
            _position1.AreaIndex = areaIndex;
        }
        else
        {
            _position2.Xyz = positionBytes;
            _position2.Angle = angle;
            _position2.AreaIndex = areaIndex;
        }
    }

    public void RestorePos(int index)
    {
        var chrPhysicsPtr = GetChrPhysicsPtr();

        byte[] xyzBytes;
        float angle;
        int savedAreaIndex;
        if (index == 0)
        {
            xyzBytes = _position1.Xyz;
            angle = _position1.Angle;
            savedAreaIndex = _position1.AreaIndex;
        }
        else
        {
            xyzBytes = _position2.Xyz;
            angle = _position2.Angle;
            savedAreaIndex = _position2.AreaIndex;
        }

        var areaIndex = memoryService.ReadInt32((IntPtr)memoryService.ReadInt64(FieldArea.Base) +
                                                FieldArea.CurrentWorldBlockIndex);

        if (areaIndex != savedAreaIndex)
        {
            Task.Run(() => { DoAreaWarp(savedAreaIndex, xyzBytes, angle); });
        }
        else
        {
            memoryService.WriteBytes(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.X, xyzBytes);
            memoryService.WriteFloat(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.Angle, angle);
        }
    }

    private void DoAreaWarp(int areaIndex, byte[] xyzBytes, float angle)
    {
        var idolId = _idolsByAreaIndex[areaIndex];

        var bytes = AsmLoader.GetAsmBytes("Warp");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (idolId, 0x0 + 2),
            (Functions.Warp, 0x10 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);

        var coordWriteHook = Hooks.SetWarpCoordinates;
        var angleWriteHook = Hooks.SetWarpAngle;

        var coordLoc = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCoords;
        var coordWriteCode = CodeCaveOffsets.Base + CodeCaveOffsets.WarpCoordsCode;

        memoryService.WriteBytes(coordLoc, xyzBytes);

        var codeBytes = AsmLoader.GetAsmBytes("WarpCoordWrite");

        bytes = AsmHelper.GetRelOffsetBytes(coordWriteCode.ToInt64(), coordLoc.ToInt64(), 7);
        Array.Copy(bytes, 0, codeBytes, 0x0 + 3, bytes.Length);
        memoryService.WriteBytes(coordWriteCode, codeBytes);

        var angleLoc = CodeCaveOffsets.Base + CodeCaveOffsets.WarpAngle;

        var angleWriteCode = CodeCaveOffsets.Base + CodeCaveOffsets.WarpAngleCode;

        var angleToWrite = new float[] { 0f, angle, 0f, 0f };
        memoryService.WriteBytes(angleLoc, MemoryMarshal.AsBytes(angleToWrite.AsSpan()).ToArray());

        codeBytes = AsmLoader.GetAsmBytes("WarpAngleWrite");
        bytes = AsmHelper.GetRelOffsetBytes(angleWriteCode.ToInt64(), angleLoc.ToInt64(), 7);
        Array.Copy(bytes, 0, codeBytes, 0x0 + 3, bytes.Length);
        memoryService.WriteBytes(angleWriteCode, codeBytes);

        hookManager.InstallHook(coordWriteCode.ToInt64(), coordWriteHook,
            [0x66, 0x0F, 0x7F, 0x80, 0xC0, 0x0A, 0x00, 0x00]);
        hookManager.InstallHook(angleWriteCode.ToInt64(), angleWriteHook,
            [0x66, 0x0F, 0x7F, 0x80, 0xD0, 0x0A, 0x00, 0x00]);

        var isGameLoadedPtr = (IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.IsLoaded;
        {
            int start = Environment.TickCount;
            while (memoryService.ReadUInt8(isGameLoadedPtr) != 0 &&
                   Environment.TickCount < start + 10000)
                Thread.Sleep(50);
        }

        {
            int start = Environment.TickCount;

            while (memoryService.ReadUInt8(isGameLoadedPtr) != 1 &&
                   Environment.TickCount < start + 10000)
                Thread.Sleep(50);
        }

        hookManager.UninstallHook(coordWriteCode.ToInt64());
        hookManager.UninstallHook(angleWriteCode.ToInt64());
    }

    public (float x, float y, float z) GetCoords()
    {
        byte[] coordBytes = memoryService.ReadBytes(GetChrPhysicsPtr() + (int)ChrIns.ChrPhysicsOffsets.X, 12);
        float x = BitConverter.ToSingle(coordBytes, 0);
        float y = BitConverter.ToSingle(coordBytes, 4);
        float z = BitConverter.ToSingle(coordBytes, 8);
        return (x, y, z);
    }

    public void SetHp(int hp) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Hp, hp);

    public int GetCurrentHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Hp);

    public int GetAttackPower()
    {
        var apPtr = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            WorldChrMan.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.AttackPower
        ], false);

        return memoryService.ReadInt32(apPtr);
    }

    public int GetMaxHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHp);

    public int GetExperience() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.PlayerGameDataOffsets.Experience);

    public void SetPosture(int posture) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Posture, posture);

    public int GetCurrentPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.Posture);

    public int GetMaxPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxPosture);

    public void AddSen(int senToAdd)
    {
        var bytes = AsmLoader.GetAsmBytes("AddSen");
        var playerGameData = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            WorldChrMan.PlayerGameData
        ], true);

        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (playerGameData.ToInt64(), 0x4 + 2),
            (senToAdd, 0xE + 2),
            (Functions.AddSen, 0x1E + 2)
        ]);

        memoryService.AllocateAndExecute(bytes);
    }

    public void Rest()
    {
        var bytes = AsmLoader.GetAsmBytes("Rest");
        var playerIns = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns
        ], true);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (playerIns.ToInt64(), 0x4 + 2),
            (Functions.Rest, 0x13 + 2)
        ]);

        memoryService.AllocateAndExecute(bytes);
    }

    public void SetAttackPower(int attackPower)
    {
        var attackPowerPointer = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            WorldChrMan.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.AttackPower
        ], false);
        memoryService.WriteInt32(attackPowerPointer, attackPower);
    }

    public void SetNewGame(int value) =>
        memoryService.WriteInt32((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame, value);

    public int GetNewGame() =>
        memoryService.ReadInt32((IntPtr)memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.NewGame);

    public void AddExperience(int experience)
    {
        var bytes = AsmLoader.GetAsmBytes("AddExperience");
        var playerGameData = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            WorldChrMan.PlayerGameData
        ], true);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (playerGameData.ToInt64(), 0x4 + 2),
            (experience, 0xE + 2),
            (Functions.AddExperience, 0x18 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void TogglePlayerNoDeath(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerNoDeath,
            isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoDeathWithoutKillbox(bool isNoDeathEnabledWithoutKillbox)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.NoDeathWithoutKillbox;
        if (isNoDeathEnabledWithoutKillbox)
        {
            var bytes = AsmLoader.GetAsmBytes("NoDeath");
            AsmHelper.WriteRelativeOffsets(bytes, [
                (code.ToInt64() + 0x1, WorldChrMan.Base.ToInt64(), 7, 0x1 + 3),
                (code.ToInt64() + 0x37, Hooks.HpWrite + 0x6, 5, 0x37 + 1)
            ]);
            memoryService.WriteBytes(code, bytes);

            hookManager.InstallHook(code.ToInt64(), Hooks.HpWrite,
                [0x89, 0x83, 0x30, 0x01, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void TogglePlayerNoDamage(bool isEnabled)
    {
        var bitFlags = GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.BitFlags;
        memoryService.SetBitValue(bitFlags, (int)ChrIns.ChrDataBitFlags.NoDamage, isEnabled);
    }

    public void TogglePlayerOneShotHealth(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerOneShotHealth, isEnabled ? 1 : 0);
    }

    public void TogglePlayerOneShotPosture(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerOneShotPosture, isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoGoodsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerNoGoodsConsume, isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoEmblemsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerNoEmblemsConsume, isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoRevivalConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerNoRevivalConsume, isEnabled ? 1 : 0);
    }

    public void TogglePlayerHide(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerHide, isEnabled ? 1 : 0);
    }

    public void TogglePlayerSilent(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + DebugFlags.PlayerSilent, isEnabled ? 1 : 0);
    }

    public void TogglePlayerInfinitePoise(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.InfinitePoise;
        if (isEnabled)
        {
            var worldChrMan = WorldChrMan.Base;
            var hookLoc = Hooks.InfinitePoise;
            var skippedStaggerLoc = hookLoc + 0x429;

            var bytes = AsmLoader.GetAsmBytes("InfinitePoise");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (code.ToInt64() + 0x1, worldChrMan.ToInt64(), 7, 0x1 + 3),
                (code.ToInt64() + 0x14, skippedStaggerLoc, 6, 0x14 + 2),
                (code.ToInt64() + 0x22, hookLoc + 8, 5, 0x22 + 1),
            });


            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), hookLoc,
                [0x4C, 0x89, 0xBC, 0x24, 0xA0, 0x00, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void ToggleInfiniteBuffs(bool isEnabled)
    {
        var infiniteConfettiCaveLoc = CodeCaveOffsets.Base + CodeCaveOffsets.InfiniteConfetti; //Base = allocated memory
        var confettiFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ConfettiFlag;
        var gachiinFlag = CodeCaveOffsets.Base + CodeCaveOffsets.GachiinFlag;
        if (isEnabled)
        {
            var hookLoc = Hooks.InfiniteConfetti; //140C06BFE
            var originalRetJmp = hookLoc + 0x7; //movss in og code

            var bytes = AsmLoader.GetAsmBytes("InfiniteConfetti");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (infiniteConfettiCaveLoc.ToInt64() + 0x19, originalRetJmp, 5, 0x19 + 1), //jmp return 1
                (infiniteConfettiCaveLoc.ToInt64() + 0x1e, confettiFlag.ToInt64(), 7, 0x1e + 2), //cmp byte     
                (infiniteConfettiCaveLoc.ToInt64() + 0x2a, originalRetJmp, 5, 0x2a + 1),
                (infiniteConfettiCaveLoc.ToInt64() + 0x2f, gachiinFlag.ToInt64(), 7, 0x2f + 2),
                (infiniteConfettiCaveLoc.ToInt64() + 0x3b, originalRetJmp, 5, 0x3b + 1),
                (infiniteConfettiCaveLoc.ToInt64() + 0x47, originalRetJmp, 5, 0x47 + 1),
            });
            memoryService.WriteBytes(infiniteConfettiCaveLoc, bytes);
            hookManager.InstallHook(infiniteConfettiCaveLoc.ToInt64(), hookLoc,
                [0xf3, 0x0f, 0x5c, 0xcf, 0x0f, 0x2f, 0xc1]);
        }
        else
        {
            hookManager.UninstallHook(infiniteConfettiCaveLoc.ToInt64());
        }
    }

    public void ToggleConfettiFlag(bool isEnabled)

    {
        var confettiFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ConfettiFlag;
        if (isEnabled)
        {
            memoryService.WriteUInt8(confettiFlag, 1);
        }
        else
        {
            memoryService.WriteUInt8(confettiFlag, 0);
        }
    }

    public void ToggleGachiinFlag(bool isEnabled)
    {
        var gachiinFlag = CodeCaveOffsets.Base + CodeCaveOffsets.GachiinFlag;
        if (isEnabled)
        {
            memoryService.WriteUInt8(gachiinFlag, 1);
        }
        else
        {
            memoryService.WriteUInt8(gachiinFlag, 0);
        }
    }

    public int RequestRespawn()
    {
        var worldBlockIdPtr = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.WorldBlockInfo,
            WorldChrMan.WorldBlockId
        ], false);
        var requestRespawnAddr = RequestRespawnGlobal.Base;
        var addr = (IntPtr)0x143afeba0;
        var worldblockid = memoryService.ReadInt32(worldBlockIdPtr);
        memoryService.WriteInt32(addr, 1101950);

        return 0;
    }

    public void RemoveSpecialEffect(int spEffect)
    {
        var bytes = AsmLoader.GetAsmBytes("RemoveSpecialEffect");
        var spEffectPtr = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            ChrIns.SpEffectManager
        ], true);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (spEffectPtr.ToInt64(), 0x4 + 2),
            (spEffect, 0xE + 2),
            (Functions.RemoveSpEffect, 0x18 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void ApplySpecialEffect(int spEffect)
    {
        var bytes = AsmLoader.GetAsmBytes("ApplySpecialEffect");
        var spEffectPtr = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns
        ], true);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (spEffectPtr.ToInt64(), 0x4 + 2),
            (spEffect, 0xE + 2),
            (Functions.ApplySpEffect, 0x18 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void ToggleDamageMultiplier(bool isEnabled)
    {
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.DamageMultiplierCode;
        if (isEnabled)
        {
            var bytes = AsmLoader.GetAsmBytes("DamageMultiplier");
            var damageMultiplier = CodeCaveOffsets.Base + CodeCaveOffsets.DamageMultiplier;
            var origin = Hooks.DamageMultiplier;
            AsmHelper.WriteRelativeOffsets(bytes, [
            (code.ToInt64() + 0x8, WorldChrMan.Base.ToInt64(), 7, 0x8 + 3),
            (code.ToInt64() + 0x2C, damageMultiplier.ToInt64(), 9, 0x2C + 5),
            (code.ToInt64() + 0x4D, damageMultiplier.ToInt64(), 9, 0x4D + 5),
            (code.ToInt64() + 0x64, origin + 7, 5, 0x64 + 1)
            ]);
            
            memoryService.WriteBytes(code, bytes);
            hookManager.InstallHook(code.ToInt64(), origin, [0x41, 0x8B, 0x96, 0xE0, 0x01, 0x00, 0x00]);
        }
        else
        {
            hookManager.UninstallHook(code.ToInt64());
        }
    }

    public void SetDamageMultiplier(float multiplier)
    {
        var damageMultiplier = CodeCaveOffsets.Base + CodeCaveOffsets.DamageMultiplier;
        memoryService.WriteFloat(damageMultiplier, multiplier);
    }

    public float GetPlayerSpeed() =>
        memoryService.ReadFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimationSpeed);

    public void SetSpeed(float speed) =>
        memoryService.WriteFloat(GetChrBehaviorPtr() + (int)ChrIns.ChrBehaviorOffsets.AnimationSpeed, speed);

    #endregion

    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

    private IntPtr GetChrPhysicsPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);

    private IntPtr GetChrBehaviorPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrBehaviorModule], true);

    #endregion
}