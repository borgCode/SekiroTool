using H.Hooks;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class SettingsViewmodel
{
    private readonly ISettingsService _settingsService;
    private readonly HotkeyManager _hotkeyManager;
    
    private readonly Dictionary<string, Action<string>> _propertySetters;
    
    private string _currentSettingHotkeyId;
    private LowLevelKeyboardHook _tempHook;
    private Keys _currentKeys;

    public SettingsViewmodel(ISettingsService settingsService, HotkeyManager hotkeyManager)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        RegisterHotkeys();
        
        
    }

    #region Properties
    
    

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        
    }

    #endregion
    
}