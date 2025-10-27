using System.IO;

namespace SekiroTool.Utilities;

public class SettingsManager
{
    private static SettingsManager _default;
    public static SettingsManager Default => _default ??= Load();
    
    public string HotkeyActionIds { get; set; } = "";
    public bool EnableHotkeys { get; set; }
    public bool NoLogo { get; set; }
    public bool AlwaysOnTop { get; set; }
    public bool NoTutorials { get; set; }
    public bool SaveInCombat { get; set; }
    public bool NoCameraSpin { get; set; }
    public bool DisableMenuMusic { get; set; }
    public bool DefaultSoundChangeEnabled { get; set; }
    public int DefaultSoundVolume { get; set; } = 3;
    
    
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
                $"NoLogo={NoLogo}",
                $"AlwaysOnTop={AlwaysOnTop}",
                $"NoTutorials={NoTutorials}",
                $"SaveInCombat={SaveInCombat}",
                $"NoCameraSpin={NoCameraSpin}",
                $"DisableMenuMusic={DisableMenuMusic}",
                $"DefaultSoundChangeEnabled={DefaultSoundChangeEnabled}",
                $"DefaultSoundVolume={DefaultSoundVolume}",
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
                            case "NoLogo":
                                bool.TryParse(value, out bool nl);
                                settings.NoLogo = nl;
                                break;
                            case "NoTutorials":
                                bool.TryParse(value, out bool nt);
                                settings.NoTutorials = nt;
                                break;
                            case "SaveInCombat":
                                bool.TryParse(value, out bool sic);
                                settings.SaveInCombat = sic;
                                break;
                            case "AlwaysOnTop":
                                bool.TryParse(value, out bool aot);
                                settings.AlwaysOnTop = aot;
                                break;
                            case "NoCameraSpin":
                                bool.TryParse(value, out bool ncp);
                                settings.NoCameraSpin = ncp;
                                break;
                            case "DisableMenuMusic":
                                bool.TryParse(value, out bool dmm);
                                settings.DisableMenuMusic = dmm;
                                break;
                            case "DefaultSoundChangeEnabled":
                                bool.TryParse(value, out bool dsce);
                                settings.DefaultSoundChangeEnabled = dsce;
                                break;
                            case "DefaultSoundVolume":
                                int.TryParse(value, out int dsv);
                                settings.DefaultSoundVolume = dsv;
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