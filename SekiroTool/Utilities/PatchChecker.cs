// 

using SekiroTool.Memory;
using SekiroTool.Services;

namespace SekiroTool.Utilities;

public static class PatchChecker
{
    
    public static bool Initialize(MemoryService memoryService)
    {
        if (memoryService.TargetProcess == null) return false;
        var module = memoryService.TargetProcess.MainModule;
        var fileVersion = module?.FileVersionInfo.FileVersion;
        var moduleBase = memoryService.BaseAddress;
        
        Console.WriteLine($@"Patch: {fileVersion}");

        return Offsets.Initialize(fileVersion, moduleBase);
    }
}