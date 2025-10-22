using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class UtilityService(IMemoryService memoryService) : IUtilityService
{
    public void OpenSkillMenu()
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuNoParams");
        AsmHelper.WriteAbsoluteAddress(bytes, Functions.OpenSkillMenu, 0x8 + 2);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenUpgradeProstheticsMenu()
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuNoParams");
        AsmHelper.WriteAbsoluteAddress(bytes, Functions.UpgradeProstheticsMenu, 0x8 + 2);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenRegularShop(ShopLineup shopLineup)
    {
        var patchLoc = Patches.OpenRegularShopPatch;
        var originalBytes = memoryService.ReadBytes(patchLoc, 5);
        memoryService.WriteBytes(patchLoc, [0xEB, 0x20, 0x90, 0x90, 0x90]);
        
        var bytes = AsmLoader.GetAsmBytes("OpenMenuTwoParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (shopLineup.StartId, 0x8 + 2),
            (shopLineup.EndId, 0x12 + 2),
            (Functions.OpenRegularShop, 0x1C +2)
        ]);
        memoryService.AllocateAndExecute(bytes);
        
        memoryService.WriteBytes(patchLoc, originalBytes);
    }

    public void OpenScalesShop(ScaleLineup scaleLineup)
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuThreeParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (scaleLineup.StartId, 0x8 + 2),
            (scaleLineup.EndId, 0x12 + 2),
            (scaleLineup.Unk, 0x1C + 2),
            (Functions.OpenScalesShop, 0x26 +2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }

    public void OpenProstheticsShop(ShopLineup shopLineup)
    {
        var bytes = AsmLoader.GetAsmBytes("OpenMenuTwoParams");
        AsmHelper.WriteAbsoluteAddresses(bytes, [
            (shopLineup.StartId, 0x8 + 2),
            (shopLineup.EndId, 0x12 + 2),
            (Functions.OpenProstheticsShop, 0x1C +2)
        ]);
        memoryService.AllocateAndExecute(bytes);
    }
}