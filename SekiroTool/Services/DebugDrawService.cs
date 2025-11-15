using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using static SekiroTool.Memory.Offsets;

namespace SekiroTool.Services;

public class DebugDrawService : IDebugDrawService
{
    private readonly IMemoryService _memoryService;
    private readonly NopManager _nopManager;
    private int _clientCount;


    public DebugDrawService(IMemoryService memoryService, IGameStateService gameStateService, NopManager nopManager)
    {
        _memoryService = memoryService;
        _nopManager = nopManager;
        gameStateService.Subscribe(GameState.Attached, Reset);
    }

    public void RequestDebugDraw()
    {
        if (_clientCount == 0) ToggleDebugDraw(true);
        _clientCount++;
    }

    public void ReleaseDebugDraw()
    {
        _clientCount--;
        
        if (_clientCount == 0) ToggleDebugDraw(false);
        
        if (_clientCount < 0) _clientCount = 0;
    }

    public int GetCount() => _clientCount;

    private void Reset()
    {
        _clientCount = 0;
        ToggleDebugDraw(false);
    }

    private void ToggleDebugDraw(bool isEnabled)
    {
        
        var flagPtr = _memoryService.ReadInt64(WorldChrManDbg.Base) + WorldChrManDbg.EnableDebugDraw;
        if (isEnabled)
        {
            _nopManager.InstallNop(Patches.DebugFont, 5);
            _memoryService.WriteUInt8((IntPtr)flagPtr, 1);
        }
        else
        {
            _memoryService.WriteUInt8((IntPtr)flagPtr, 0);
       
        }
    }
}