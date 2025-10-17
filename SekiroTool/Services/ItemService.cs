using System.Diagnostics;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Models;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class ItemService(IMemoryService memoryService) : IItemService
{
    public void SpawnItem(Item item, int quantity)
    {
        var structPtr = CodeCaveOffsets.Base + CodeCaveOffsets.ItemStruct;
        var code = CodeCaveOffsets.Base + CodeCaveOffsets.ItemGiveCode;


        var bytes = AsmLoader.GetAsmBytes("GiveItem");

        AsmHelper.WriteRelativeOffsets(bytes, new[]
        {
            (code.ToInt64() + 0x4, MapItemMan.Base.ToInt64(), 7, 0x4 + 3),
            (code.ToInt64() + 0xB, structPtr.ToInt64(), 7, 0xB + 3),
            (code.ToInt64() + 0x22, Functions.ItemSpawn, 5, 0x22 + 1)
        });
        
        memoryService.WriteInt32(structPtr, 1);
        memoryService.WriteUInt16(structPtr + 0x4, item.ItemType);
        memoryService.WriteUInt16(structPtr + 0x6, item.ItemId);
        memoryService.WriteInt32(structPtr + 0x8, quantity);
        memoryService.WriteBytes(code, bytes);

        memoryService.RunThread(code);
    }
}