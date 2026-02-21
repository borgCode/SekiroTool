using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class ChrInsService(IMemoryService memoryService) : IChrInsService
{

    #region Public Methods
    
    public nint GetChrInsByEntityId(int entityId)
    {
        memoryService.Write(CodeCaveOffsets.Base + CodeCaveOffsets.EntityIdInput, entityId);
        
        var bytes = AsmLoader.GetAsmBytes(AsmScript.GetChrInsByEntityId);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (CodeCaveOffsets.Base + CodeCaveOffsets.EntityIdInput, 0x0 + 2),
            (Functions.GetChrInsByEntityId, 0xa + 2),
            (CodeCaveOffsets.Base + CodeCaveOffsets.ChrInsByEntityIdResult,0x1e + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
        
        return memoryService.Read<nint>(CodeCaveOffsets.Base + CodeCaveOffsets.ChrInsByEntityIdResult);
    }

    public void SetHp(nint chrIns, int hp) =>
        memoryService.Write(GetChrDataPtr(chrIns) + (int)ChrIns.ChrDataOffsets.Hp, hp);

    public void SetHpNode(nint chrIns, int nodeNum) =>
        memoryService.Write(GetChrDataPtr(chrIns) + (int)ChrIns.ChrDataOffsets.CurrentBossNode, nodeNum);

    #endregion


    #region Private Methods

    private nint GetChrDataPtr(nint chrIns) =>
        memoryService.FollowPointers(chrIns, ChrIns.ChrDataModule, true, false);

    #endregion
}