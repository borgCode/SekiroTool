using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class ChrInsService(IMemoryService memoryService) : IChrInsService
{

    #region Public Methods
    
    public IntPtr GetChrInsByEntityId(int entityId)
    {
        memoryService.WriteInt32(CodeCaveOffsets.Base + CodeCaveOffsets.EntityIdInput, entityId);
        
        var bytes = AsmLoader.GetAsmBytes("GetChrInsByEntityId");
        AsmHelper.WriteAbsoluteAddresses(bytes, new[]
        {
            (CodeCaveOffsets.Base.ToInt64() + CodeCaveOffsets.EntityIdInput, 0x0 + 2),
            (Offsets.Functions.GetChrInsByEntityId, 0xa + 2),
            (CodeCaveOffsets.Base.ToInt64()+ CodeCaveOffsets.ChrInsByEntityIdResult,0x1e + 2)
        });
        memoryService.AllocateAndExecute(bytes);
        
        return (IntPtr) memoryService.ReadInt64(CodeCaveOffsets.Base + CodeCaveOffsets.ChrInsByEntityIdResult);
    }
    
    #endregion
}