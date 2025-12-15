// 

using System.IO;
using SekiroTool.Enums;

namespace SekiroTool.Utilities;

public static class PatchChecker
{
    private static readonly Dictionary<long, Patch> PatchSizes = new()
    {
        [65682008] = Patch.V102,
        [65688152] = Patch.V103and4,
        [68005144] = Patch.V106,
        [68002072] = Patch.V105
    };

    public static Patch? CurrentPatch { get; private set; }

    public static void Initialize(string exePath)
    {
        if (!File.Exists(exePath))
        {
            CurrentPatch = null;
            return;
        }

        var fileSize = new FileInfo(exePath).Length;
        Console.WriteLine(fileSize);
        CurrentPatch = PatchSizes.TryGetValue(fileSize, out var patch) ? patch : null;
    }

    public static bool IsInitialized => CurrentPatch.HasValue;
}