// 

using System.IO;
using SekiroTool.Enums;

namespace SekiroTool.Utilities;

public static class PatchChecker
{
    private static readonly Dictionary<long, Patch> PatchSizes = new()
    {
        [67727360] = Patch.V102,
        [67731456] = Patch.V104,
        [70066176] = Patch.V106
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