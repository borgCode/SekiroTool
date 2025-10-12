using System.Text.RegularExpressions;

namespace SekiroTool.Utilities
{
    public static class AsmLoader
    {
        private const string BytePattern = @"^(?:[\da-f]{2} )*(?:[\da-f]{2}(?=\s|$))";

        internal static byte[] GetAsmBytes(string resourceName)
        {
            string asmFile = GetResourceContent(resourceName);
            return ParseBytes(asmFile);
        }

        private static string GetResourceContent(string resourceName)
        {
            object? resource = Resources.ResourceManager.GetObject(resourceName);
            return resource as string ??
                   throw new ArgumentException($"Resource '{resourceName}' not found or is not a string.");
        }

        private static byte[] ParseBytes(string asmFile)
        {
            return Regex.Matches(asmFile, BytePattern, RegexOptions.IgnoreCase | RegexOptions.Multiline)
                .Cast<Match>()
                .SelectMany(m => m.Value.Split(' '))
                .Select(hex => Convert.ToByte(hex, 16))
                .ToArray();
        }
    }
}