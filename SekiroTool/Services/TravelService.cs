using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Models;

namespace SekiroTool.Services;

public class TravelService(IMemoryService memoryService, HookManager hookManager) : ITravelService
{
    public void Warp(Warp warp)
    {
        throw new NotImplementedException();
    }

    public void UnlockAllIdols()
    {
        throw new NotImplementedException();
    }
}