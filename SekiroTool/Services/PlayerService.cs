using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class PlayerService(IMemoryService memoryService, HookManager hookManager) : IPlayerService
{
    #region Public Methods

    public void SavePos(int index)
    {
        var chrPhysicsPtr = GetChrPhysicsPtr();

        byte[] positionBytes = memoryService.ReadBytes(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.X, 12);
        float angle = memoryService.ReadFloat(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.Angle);

        byte[] angleBytes = BitConverter.GetBytes(angle);
        byte[] data = new byte[16];
        Buffer.BlockCopy(positionBytes, 0, data, 0, 12);
        Buffer.BlockCopy(angleBytes, 0, data, 12, 4);

        if (index == 0) memoryService.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.SavePos1, data);
        else memoryService.WriteBytes(CodeCaveOffsets.Base + CodeCaveOffsets.SavePos2, data);
    }

    public void RestorePos(int index)
    {
        byte[] positionBytes;
        if (index == 0) positionBytes = memoryService.ReadBytes(CodeCaveOffsets.Base + CodeCaveOffsets.SavePos1, 16);
        else positionBytes = memoryService.ReadBytes(CodeCaveOffsets.Base + CodeCaveOffsets.SavePos2, 16);

        float angle = BitConverter.ToSingle(positionBytes, 12);

        var chrPhysicsPtr = GetChrPhysicsPtr();

        byte[] xyzBytes = new byte[12];
        Buffer.BlockCopy(positionBytes, 0, xyzBytes, 0, 12);

        memoryService.WriteBytes(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.X, xyzBytes);
        memoryService.WriteFloat(chrPhysicsPtr + (int)ChrIns.ChrPhysicsOffsets.Angle, angle);
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

    public int GetMaxHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)ChrIns.ChrDataOffsets.MaxHp);

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
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerNoDeath, isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoDamage(bool isEnabled)
    {
    }

    public void TogglePlayerOneShotHealth(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerOneShotHealth, isEnabled ? 1 : 0);
    }

    public void TogglePlayerOneShotPosture(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerOneShotPosture,
            isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoGoodsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerNoGoodsConsume,
            isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoEmblemsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerNoEmblemsConsume,
            isEnabled ? 1 : 0);
    }

    public void TogglePlayerNoRevivalConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerNoRevivalConsume,
            isEnabled ? 1 : 0);
    }

    public void TogglePlayerHide(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerHide, isEnabled ? 1 : 0);
    }

    public void TogglePlayerSilent(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int)DebugFlags.Flag.PlayerSilent, isEnabled ? 1 : 0);
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

    public void ToggleInfiniteConfetti(bool isEnabled)
    {
        var infiniteConfettiCaveLoc  = CodeCaveOffsets.Base + CodeCaveOffsets.InfiniteConfetti; //Base = allocated memory
        var confettiFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ConfettiFlag;
        var gachiinFlag = CodeCaveOffsets.Base + CodeCaveOffsets.GachiinFlag;
        if (isEnabled)
        {
            var hookLoc = Hooks.InfiniteConfetti; //140C06BFE
            var originalRetJmp = hookLoc + 0x7;  //movss in og code

            var bytes = AsmLoader.GetAsmBytes("InfiniteConfetti");
            AsmHelper.WriteRelativeOffsets(bytes, new[]
            {
                (infiniteConfettiCaveLoc.ToInt64() + 0x19, originalRetJmp, 5, 0x19 + 1),  //jmp return 1
                (infiniteConfettiCaveLoc.ToInt64() + 0x1e, confettiFlag.ToInt64(), 7, 0x1e + 2), //cmp byte     
                (infiniteConfettiCaveLoc.ToInt64() + 0x2a, originalRetJmp, 5, 0x2a + 1),
                (infiniteConfettiCaveLoc.ToInt64() + 0x2f, gachiinFlag.ToInt64(), 7, 0x2f + 2),
                (infiniteConfettiCaveLoc.ToInt64() + 0x3b, originalRetJmp, 5, 0x3b + 1),  
                (infiniteConfettiCaveLoc.ToInt64() + 0x47, originalRetJmp, 5, 0x47 + 1),  
            });
            memoryService.WriteBytes(infiniteConfettiCaveLoc, bytes);
            
            memoryService.WriteUInt8(gachiinFlag, 1);
            hookManager.InstallHook(infiniteConfettiCaveLoc.ToInt64(), hookLoc,
                [0xf3, 0x0f, 0x5c, 0xcf, 0x0f, 0x2f, 0xc1]);
        }
        else
        {
            hookManager.UninstallHook(infiniteConfettiCaveLoc.ToInt64());
        }
    }

    public void ToggleConfettiFlag(bool isEnabled)
    
    {   var confettiFlag = CodeCaveOffsets.Base + CodeCaveOffsets.ConfettiFlag;
        if (isEnabled)
        {
            memoryService.WriteUInt8(confettiFlag, 1);
        }
            
            
    }

    public void ToggleGachiinFlag(bool isEnabled)
    {
        var gachiinFlag = CodeCaveOffsets.Base + CodeCaveOffsets.GachiinFlag;
        if (isEnabled)
        {
            memoryService.WriteUInt8(gachiinFlag, 1);
        }  
    }

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

    private IntPtr GetChrPhysicsPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrPhysicsModule], true);

    #endregion
}