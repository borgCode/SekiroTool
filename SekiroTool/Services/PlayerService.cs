using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class PlayerService(IMemoryService memoryService) : IPlayerService
{
    #region Public Methods

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
        var playerIns = memoryService.FollowPointers(WorldChrMan.Base,[
        WorldChrMan.PlayerIns],true);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (playerIns.ToInt64(), 0x4 + 2),
            (Functions.Rest, 0x13 + 2)]);
        
        memoryService.AllocateAndExecute(bytes);
    }

    public void SetAttackPower(int attackPower)
    {
        var attackPowerPointer = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            WorldChrMan.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.AttackPower], false);
        memoryService.WriteInt32(attackPowerPointer, attackPower);
    }

    public void AddExperience(int experience)
    {
       var bytes = AsmLoader.GetAsmBytes("AddExperience");
       var playerGameData = memoryService.FollowPointers(WorldChrMan.Base, [
       WorldChrMan.PlayerIns,
       WorldChrMan.PlayerGameData],true);
       AsmHelper.WriteAbsoluteAddresses(bytes, [
           (playerGameData.ToInt64(), 0x4 + 2),
           (experience, 0xE + 2),
           (Functions.AddExperience, 0x18 + 2)
       ]);
       memoryService.AllocateAndExecute(bytes);
    }

    public void TogglePlayerNoDeath(bool isEnabled)
        { 
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerNoDeath, (byte)(isEnabled ? 1 : 0));
        }

    public void TogglePlayerOneShotHealth(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerOneShotHealth, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerOneShotPosture(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerOneShotPosture, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerNoGoodsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerNoGoodsConsume, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerNoEmblemsConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerNoEmblemsConsume, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerNoRevivalConsume(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerNoRevivalConsume, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerHide(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerHide, (byte)(isEnabled ? 1 : 0));
    }

    public void TogglePlayerSilent(bool isEnabled)
    {
        memoryService.WriteUInt8(DebugFlags.Base + (int) DebugFlags.Flag.PlayerSilent, (byte)(isEnabled ? 1 : 0));
    }

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

    #endregion
}