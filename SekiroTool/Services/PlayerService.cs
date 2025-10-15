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
            ChrIns.PlayerGameData
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
            (Functions.Rest, 0x13 + 2)
        ]);
        
        memoryService.AllocateAndExecute(bytes);
    }

    public void SetAttackPower(int attackPower)
    {
        var attackPowerPointer = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            ChrIns.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.AttackPower], false);
        memoryService.WriteInt32(attackPowerPointer, attackPower);
    }

    public void AddExperience(int experience)
    {
        var experiencePointer = memoryService.FollowPointers(WorldChrMan.Base, [
            WorldChrMan.PlayerIns,
            ChrIns.PlayerGameData,
            (int)ChrIns.PlayerGameDataOffsets.Experience], false);
        memoryService.WriteInt32(experiencePointer,experience);
    }

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, [WorldChrMan.PlayerIns, ..ChrIns.ChrDataModule], true);

    #endregion
}