using System.IO;

namespace SekiroTool.Utilities;

public class SettingsManager
{
    private static SettingsManager _default;
    public static SettingsManager Default => _default ??= Load();
    
    public string HotkeyActionIds { get; set; } = "";
    public bool EnableHotkeys { get; set; }

    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SekiroTool",
        "settings.txt");
    
    
    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                
            var lines = new[]
            {
                $"HotkeyActionIds={HotkeyActionIds}",
                $"EnableHotkeys={EnableHotkeys}",
            };

            File.WriteAllLines(SettingsPath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }


    private static SettingsManager Load()
    {
        var settings = new SettingsManager();

        if (File.Exists(SettingsPath))
        {
            try
            {
                foreach (var line in File.ReadAllLines(SettingsPath))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0];
                        var value = parts[1];

                        switch (key)
                        {
                            case "HotkeyActionIds": settings.HotkeyActionIds = value; break;
                            case "EnableHotkeys":
                                bool.TryParse(value, out bool eh);
                                settings.EnableHotkeys = eh;
                                break;
                        }
                    }
                }
            }
            catch
            {
                // Return default settings on error
            }
        }

        return settings;
    }
}