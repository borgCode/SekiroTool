using SekiroTool.Interfaces;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class SettingsService(IMemoryService memoryService) : ISettingsService
{
    public void Quitout() =>
        memoryService.WriteUInt8((IntPtr)memoryService.ReadInt64(MenuMan.Base) + MenuMan.Quitout, 1);
}