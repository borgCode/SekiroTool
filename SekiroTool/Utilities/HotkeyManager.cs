using H.Hooks;
using SekiroTool.Interfaces;

namespace SekiroTool.Utilities;

public class HotkeyManager
{
    private readonly IMemoryService _memoryService;
    private readonly LowLevelKeyboardHook _keyboardHook = new();
    private readonly Dictionary<string, Keys> _hotkeyMappings = new();
    private readonly Dictionary<string, Action> _actions = new();

    public HotkeyManager(IMemoryService memoryService)
    {
        _memoryService = memoryService;
        
        _keyboardHook.HandleModifierKeys = true;
        _keyboardHook.Down += KeyboardHook_Down;
        LoadHotkeys();
        // if (SettingsManager.Default.EnableHotkeys) _keyboardHook.Start();
    }
    
    public void Start()
    {
        _keyboardHook.Start();
    }
        
    public void Stop()
    {
        _keyboardHook.Stop();
    }
    
    public void RegisterAction(string actionId, Action action)
    {
        _actions[actionId] = action;
    }
    
    
    private void KeyboardHook_Down(object sender, KeyboardEventArgs e)
    {
        if (!IsGameFocused())
            return;
        foreach (var mapping in _hotkeyMappings)
        {
            string actionId = mapping.Key;
            Keys keys = mapping.Value;
            if (!e.Keys.Are(keys.Values.ToArray())) continue;
            if (_actions.TryGetValue(actionId, out var action))
            {
                action.Invoke();
            }
            break;
        }
    }
    
    private bool IsGameFocused()
    {
        if (_memoryService.TargetProcess == null || _memoryService.TargetProcess.Id == 0) return false;
         
        IntPtr foregroundWindow = User32.GetForegroundWindow();
        User32.GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
        return foregroundProcessId == (uint)_memoryService.TargetProcess.Id;
    }
    
    public void SetHotkey(string actionId, Keys keys)
    {
        _hotkeyMappings[actionId] = keys;
        SaveHotkeys();
    }
        
    public void ClearHotkey(string actionId)
    {
        _hotkeyMappings.Remove(actionId);
        SaveHotkeys();
    }
    

   public Keys GetHotkey(string actionId)
        {
            return _hotkeyMappings.TryGetValue(actionId, out var keys) ? keys : null;
        }
        
        public string GetActionIdByKeys(Keys keys)
        {
            return _hotkeyMappings.FirstOrDefault(x => x.Value == keys).Key;
        }
        
        
        public void SaveHotkeys()
        {
            try
            {
                var mappingPairs = new List<string>();
        
                foreach (var mapping in _hotkeyMappings)
                {
                    mappingPairs.Add($"{mapping.Key}={mapping.Value}");
                }
                
                SettingsManager.Default.HotkeyActionIds = string.Join(";", mappingPairs);
        
                SettingsManager.Default.Save();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving hotkeys: {ex.Message}");
            }
        }

        public void LoadHotkeys()
        {
            try
            {
                _hotkeyMappings.Clear();
        
                string mappingsString = SettingsManager.Default.HotkeyActionIds;
        
                if (!string.IsNullOrEmpty(mappingsString))
                {
                    string[] pairs = mappingsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            
                    foreach (string pair in pairs)
                    {
                        int separatorIndex = pair.IndexOf('=');
                        if (separatorIndex > 0)
                        {
                            string actionId = pair.Substring(0, separatorIndex);
                            string keyValue = pair.Substring(separatorIndex + 1);
                    
                            _hotkeyMappings[actionId] = Keys.Parse(keyValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading hotkeys: {ex.Message}");
            }
        }
}