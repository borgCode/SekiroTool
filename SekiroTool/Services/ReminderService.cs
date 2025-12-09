// 

using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class ReminderService(IMemoryService memoryService) : IReminderService
{
    public void ChangeIdolIcon()
    {
        var bytes = AsmLoader.GetAsmBytes("ChangeIdolIcon");
        AsmHelper.WriteAbsoluteAddress(bytes, Functions.GetGoodsParam, 0xE + 2);
        memoryService.AllocateAndExecute(bytes);
    }
}