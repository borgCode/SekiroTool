using System.Diagnostics;
using SekiroTool.Enums;
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


        var bytes = AsmLoader.GetAsmBytes(AsmScript.GiveItem);

        AsmHelper.WriteRelativeOffsets(bytes, [
            (code + 0x4, MapItemMan.Base, 7, 0x4 + 3),
            (code + 0xB, structPtr, 7, 0xB + 3),
            (code + 0x22, Functions.ItemSpawn, 5, 0x22 + 1)
        ]);
        
        memoryService.Write(structPtr, 1);
        memoryService.Write(structPtr + 0x4, (short) item.ItemId);
        memoryService.Write(structPtr + 0x6, item.ItemType);
        memoryService.Write(structPtr + 0x8, quantity);
        memoryService.WriteBytes(code, bytes);

        memoryService.RunThread(code);
    }

    public void GiveSkillOrPros(int id)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.GiveSkillsAndPros);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            ( id, 0x4 + 2),
            (Functions.GiveSkillAndPros, 0x1A + 2)
        ]);
        
        memoryService.AllocateAndExecute(bytes);
    }

    public void RemoveItem(int id)
    {
        var bytes = AsmLoader.GetAsmBytes(AsmScript.RemoveItem);
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (GameDataMan.Base, 0x0 + 2),
            (id, 0x18 + 2),
            (Functions.GetItemSlot, 0x2F + 2),
            (Functions.RemoveItem, 0x50 + 2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }
}