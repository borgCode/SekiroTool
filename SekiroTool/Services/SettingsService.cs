using SekiroTool.Interfaces;
using SekiroTool.Memory;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class SettingsService(IMemoryService memoryService, NopManager nopManager) : ISettingsService
{
    public void Quitout() =>
        memoryService.WriteUInt8((IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.Quitout, 1);

    public void ToggleNoLogo(bool isEnabled) => memoryService.WriteBytes(Patches.NoLogo, isEnabled ? [0xEB] : [0x74]);
    
    public void ToggleNoTutorials(bool isEnabled)
    {
        if (isEnabled)
        {
            nopManager.InstallNop(Patches.MenuTutorialSkip, 4);
            nopManager.InstallNop(Patches.ShowSmallHintBox, 5);
            nopManager.InstallNop(Patches.ShowTutorialText, 5);
        }
        else
        {
            nopManager.RestoreNop(Patches.MenuTutorialSkip);
            nopManager.RestoreNop(Patches.ShowSmallHintBox);
            nopManager.RestoreNop(Patches.ShowSmallHintBox);
        }
    }
}

