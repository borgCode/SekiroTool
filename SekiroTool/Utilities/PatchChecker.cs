// 

using System.IO;
using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.Utilities;

public static class PatchChecker
{
    private static readonly Dictionary<long, Patch> PatchSizes = new()
    {
        [65682008] = Patch.Version1_2_0,
        [65688152] = Patch.Version1_3_0,
        [68005144] = Patch.Version1_6_0,
        [68002072] = Patch.Version1_5_0
    };

    public static Patch? CurrentPatch { get; private set; }

    public static void Initialize(string exePath, IMemoryService memoryService)
    {
        if (!File.Exists(exePath))
        {
            CurrentPatch = null;
            return;
        }
        
        var module = memoryService.TargetProcess.MainModule;
        var fileVersion = module?.FileVersionInfo.FileVersion;
        var moduleBase = memoryService.BaseAddress;

        Console.WriteLine($@"Patch: {fileVersion}");

        var fileSize = new FileInfo(exePath).Length;
        Console.WriteLine(fileSize);
        CurrentPatch = PatchSizes.TryGetValue(fileSize, out var patch) ? patch : null;
    }

    public static bool IsInitialized => CurrentPatch.HasValue;
}