using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Win32;

namespace SekiroTool.Utilities
{
    public static class GameLauncher
    {
        
        public static void LaunchSekiro()
        {
            try
            {
                
                
                string exePath = GetSekiroExePath();
                if (exePath == null)
                {
                    return;
                }

                var process = new Process { StartInfo = new ProcessStartInfo(exePath) };
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch Sekiro: {ex.Message}",
                    "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private static string GetSekiroExePath()
        {
            try
            {
                string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SekiroTool");
                string configFile = Path.Combine(appDataPath, "config.txt");

                if (File.Exists(configFile))
                {
                    string savedPath = File.ReadAllText(configFile);
                    if (File.Exists(savedPath))
                        return savedPath;
                }
                
                string steamPath =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath",
                        null) as string;
                if (string.IsNullOrEmpty(steamPath))
                    throw new FileNotFoundException("Steam installation path not found in registry.");

                string configPath = Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
                if (!File.Exists(configPath))
                    throw new FileNotFoundException($"Steam library configuration not found at {configPath}");

                var paths = new List<string> { steamPath };
                var regex = new Regex(@"""path""\s*""(.+?)""");

                foreach (var line in File.ReadLines(configPath))
                {
                    var match = regex.Match(line);
                    if (match.Success) paths.Add(match.Groups[1].Value.Replace(@"\\", @"\"));
                }

                foreach (var path in paths)
                {
                    string fullPath = Path.Combine(path, @"steamapps\common\Sekiro\sekiro.exe");
                    if (File.Exists(fullPath)) return fullPath;
                }

                var result = MessageBox.Show(
                    "Sekiro executable could not be found automatically.\n\n" +
                    "Please select Sekiro.exe manually.\n\n",
                    "Executable Not Found",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.OK)
                {
                    var openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Sekiro Executable",
                        Filter = "Executable Files (*.exe)|*.exe",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };

                    if (openFileDialog.ShowDialog() != true) return null;
                    string selectedPath = openFileDialog.FileName;
                    if (Path.GetFileName(selectedPath).Equals("Sekiro.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.CreateDirectory(appDataPath);
                        File.WriteAllText(configFile, selectedPath);
                        return selectedPath;
                    }

                    MessageBox.Show("Please select Sekiro.exe.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                return null;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding Sekiro: {ex.Message}", "Game Not Found", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                return null;
            }
        }
    }
}