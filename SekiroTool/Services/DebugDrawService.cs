using SekiroTool.Interfaces;
using SekiroTool.Memory;

namespace SekiroTool.Services;

public class DebugDrawService(MemoryService memoryService) : IDebugDrawService
{
    private int _clientCount;
    
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

    public void Reset()
    {
        _clientCount = 0;
        ToggleDebugDraw(false);
    }

    private void ToggleDebugDraw(bool isEnabled)
    {
        var flagPtr = memoryService.ReadInt64(Offsets.WorldChrManDbg.Base) + Offsets.WorldChrManDbg.EnableDebugDraw;
        memoryService.WriteUInt8((IntPtr)flagPtr, (byte)(isEnabled ? 1 : 0));
    }
}