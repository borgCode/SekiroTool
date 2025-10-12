using SekiroTool.Interfaces;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class PlayerService(IMemoryService memoryService) : IPlayerService
{
    #region Public Methods

    public void SetHp(int hp) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.Hp, hp);

    public int GetCurrentHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.Hp);

    public int GetMaxHp() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.MaxHp);

    public void SetPosture(int posture) =>
        memoryService.WriteInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.Posture, posture);

    public int GetCurrentPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.Posture);

    public int GetMaxPosture() =>
        memoryService.ReadInt32(GetChrDataPtr() + (int)WorldChrMan.ChrDataOffsets.MaxPosture);

    #endregion


    #region Private Methods

    private IntPtr GetChrDataPtr() =>
        memoryService.FollowPointers(WorldChrMan.Base, WorldChrMan.ChrDataModule, true);

    #endregion
}