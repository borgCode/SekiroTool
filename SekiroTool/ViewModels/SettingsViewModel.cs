using H.Hooks;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly HotkeyManager _hotkeyManager;
    
    private readonly Dictionary<string, Action<string>> _propertySetters;
    
    private string _currentSettingHotkeyId;
    private LowLevelKeyboardHook _tempHook;
    private Keys _currentKeys;

    public SettingsViewModel(ISettingsService settingsService, HotkeyManager hotkeyManager)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        RegisterHotkeys();
        
        _propertySetters = new Dictionary<string, Action<string>>
            {
                // { "SavePos1", text => SavePos1HotkeyText = text },
                // { "SavePos2", text => SavePos2HotkeyText = text },
                // { "RestorePos1", text => RestorePos1HotkeyText = text },
                // { "RestorePos2", text => RestorePos2HotkeyText = text },
                // { "RTSR", text => RtsrHotkeyText = text },
                // { "NoDeath", text => NoDeathHotkeyText = text },
                // { "OneShot", text => OneShotHotkeyText = text },
                // { "PlayerNoDamage", text => NoDamagePlayerHotkeyText = text },
                // { "TogglePlayerSpeed", text => TogglePlayerSpeedHotkeyText = text },
                // { "IncreasePlayerSpeed", text => IncreasePlayerSpeedHotkeyText = text },
                // { "DecreasePlayerSpeed", text => DecreasePlayerSpeedHotkeyText = text },
                // { "DealNoDamage", text => DealNoDamageHotkeyText = text },
                // { "RestoreSpellcasts", text => RestoreSpellcastsHotkeyText = text },
                // { "RestoreHumanity", text => RestoreHumanityHotkeyText = text },
                // { "Rest", text => RestHotkeyText = text },
                // { "ToggleGameSpeed", text => ToggleGameSpeedHotkeyText = text },
                // { "IncreaseGameSpeed", text => IncreaseGameSpeedHotkeyText = text },
                // { "DecreaseGameSpeed", text => DecreaseGameSpeedHotkeyText = text },
                // { "NoClip", text => NoClipHotkeyText = text },
                { HotkeyActions.Quitout.ToString(), text => QuitoutHotkeyText = text },
                // { "ForceSave", text => ForceSaveHotkeyText = text },
                // { "EnableFreeCam", text => EnableFreeCamHotkeyText = text },
                // { "MoveCamToPlayer", text => MoveCamToPlayerHotkeyText = text },
                // { "Warp", text => WarpHotkeyText = text },
                // { "IncreaseNoClipSpeed", text => IncreaseNoClipSpeedHotkeyText = text },
                // { "DecreaseNoClipSpeed", text => DecreaseNoClipSpeedHotkeyText = text },
                { HotkeyActions.EnableTargetOptions.ToString(), text => EnableTargetOptionsHotkeyText = text },
                { HotkeyActions.FreezeTargetHp.ToString(), text => FreezeHpHotkeyText = text },
                { HotkeyActions.SetTargetOneHp.ToString(), text => SetTargetOneHpHotkeyText = text },
                { HotkeyActions.TargetCustomHp.ToString(), text => TargetCustomHpHotkeyText = text },
                { HotkeyActions.FreezeTargetPosture.ToString(), text => FreezeTargetPostureHotkeyText = text },
                { HotkeyActions.SetTargetOnePosture.ToString(), text => SetTargetOnePostureHotkeyText = text },
                { HotkeyActions.TargetCustomPosture.ToString(), text => TargetCustomPostureHotkeyText = text },
                { HotkeyActions.ShowAllResistances.ToString(), text => ShowAllResistancesHotkeyText = text },
                { HotkeyActions.RepeatAct.ToString(), text => RepeatActHotkeyText = text },
                { HotkeyActions.RepeatKengekiAct.ToString(), text => RepeatKengekiActHotkeyText = text },
                { HotkeyActions.IncreaseTargetSpeed.ToString(), text => IncreaseTargetSpeedHotkeyText = text },
                { HotkeyActions.DecreaseTargetSpeed.ToString(), text => DecreaseTargetSpeedHotkeyText = text },
                { HotkeyActions.ToggleTargetSpeed.ToString(), text => ToggleTargetSpeedHotkeyText = text },
                { HotkeyActions.FreezeTargetAi.ToString(), text => FreezeTargetAiHotkeyText = text },
                { HotkeyActions.NoAttackTargetAi.ToString(), text => NoAttackTargetAiHotkeyText = text },
                { HotkeyActions.NoMoveTargetAi.ToString(), text => NoMoveTargetAiHotkeyText = text },
                { HotkeyActions.TargetNoPostureBuildup.ToString(), text => TargetNoPostureBuildupHotkeyText = text },
                { HotkeyActions.TargetNoDeath.ToString(), text => TargetNoDeathHotkeyText = text },
                { HotkeyActions.TargetTargetingView.ToString(), text => TargetTargetingViewHotkeyText = text },
                // { "AllNoDeath", text => AllNoDeathHotkeyText = text },
                // { "AllNoDamage", text => AllNoDamageHotkeyText = text },
                // { "AllRepeatAct", text => AllRepeatActHotkeyText = text },
               
            };
            
            LoadHotkeyDisplays();
    }

    #region Properties
    
    private bool _isEnableHotkeysEnabled;
    public bool IsEnableHotkeysEnabled
    {
        get => _isEnableHotkeysEnabled;
        set
        {
            if (SetProperty(ref _isEnableHotkeysEnabled, value))
            {
                SettingsManager.Default.EnableHotkeys = value;
                SettingsManager.Default.Save();
                if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
                else _hotkeyManager.Stop();
            }
        }
    }
    
    
    private string _quitoutHotkeyText;
    public string QuitoutHotkeyText
    {
        get => _quitoutHotkeyText;
        set => SetProperty(ref _quitoutHotkeyText, value);
    }


    private string _enableTargetOptionsHotkeyText;
    public string EnableTargetOptionsHotkeyText
    {
        get => _enableTargetOptionsHotkeyText;
        set => SetProperty(ref _enableTargetOptionsHotkeyText, value);
    }
    
    private string _freezeHpHotkeyText;
    public string FreezeHpHotkeyText
    {
        get => _freezeHpHotkeyText;
        set => SetProperty(ref _freezeHpHotkeyText, value);
    }
    
    private string _setTargetOneHpHotkeyText;
public string SetTargetOneHpHotkeyText
{
    get => _setTargetOneHpHotkeyText;
    set => SetProperty(ref _setTargetOneHpHotkeyText, value);
}

private string _targetCustomHpHotkeyText;
public string TargetCustomHpHotkeyText
{
    get => _targetCustomHpHotkeyText;
    set => SetProperty(ref _targetCustomHpHotkeyText, value);
}

private string _freezeTargetPostureHotkeyText;
public string FreezeTargetPostureHotkeyText
{
    get => _freezeTargetPostureHotkeyText;
    set => SetProperty(ref _freezeTargetPostureHotkeyText, value);
}

private string _setTargetOnePostureHotkeyText;
public string SetTargetOnePostureHotkeyText
{
    get => _setTargetOnePostureHotkeyText;
    set => SetProperty(ref _setTargetOnePostureHotkeyText, value);
}

private string _targetCustomPostureHotkeyText;
public string TargetCustomPostureHotkeyText
{
    get => _targetCustomPostureHotkeyText;
    set => SetProperty(ref _targetCustomPostureHotkeyText, value);
}

private string _showAllResistancesHotkeyText;
public string ShowAllResistancesHotkeyText
{
    get => _showAllResistancesHotkeyText;
    set => SetProperty(ref _showAllResistancesHotkeyText, value);
}

private string _repeatActHotkeyText;
public string RepeatActHotkeyText
{
    get => _repeatActHotkeyText;
    set => SetProperty(ref _repeatActHotkeyText, value);
}

private string _repeatKengekiActHotkeyText;
public string RepeatKengekiActHotkeyText
{
    get => _repeatKengekiActHotkeyText;
    set => SetProperty(ref _repeatKengekiActHotkeyText, value);
}

private string _increaseTargetSpeedHotkeyText;
public string IncreaseTargetSpeedHotkeyText
{
    get => _increaseTargetSpeedHotkeyText;
    set => SetProperty(ref _increaseTargetSpeedHotkeyText, value);
}

private string _decreaseTargetSpeedHotkeyText;
public string DecreaseTargetSpeedHotkeyText
{
    get => _decreaseTargetSpeedHotkeyText;
    set => SetProperty(ref _decreaseTargetSpeedHotkeyText, value);
}

private string _toggleTargetSpeedHotkeyText;
public string ToggleTargetSpeedHotkeyText
{
    get => _toggleTargetSpeedHotkeyText;
    set => SetProperty(ref _toggleTargetSpeedHotkeyText, value);
}

private string _freezeTargetAiHotkeyText;
public string FreezeTargetAiHotkeyText
{
    get => _freezeTargetAiHotkeyText;
    set => SetProperty(ref _freezeTargetAiHotkeyText, value);
}

private string _noAttackTargetAiHotkeyText;
public string NoAttackTargetAiHotkeyText
{
    get => _noAttackTargetAiHotkeyText;
    set => SetProperty(ref _noAttackTargetAiHotkeyText, value);
}

private string _noMoveTargetAiHotkeyText;
public string NoMoveTargetAiHotkeyText
{
    get => _noMoveTargetAiHotkeyText;
    set => SetProperty(ref _noMoveTargetAiHotkeyText, value);
}

private string _targetNoPostureBuildupHotkeyText;
public string TargetNoPostureBuildupHotkeyText
{
    get => _targetNoPostureBuildupHotkeyText;
    set => SetProperty(ref _targetNoPostureBuildupHotkeyText, value);
}

private string _targetNoDeathHotkeyText;
public string TargetNoDeathHotkeyText
{
    get => _targetNoDeathHotkeyText;
    set => SetProperty(ref _targetNoDeathHotkeyText, value);
}

private string _targetTargetingViewHotkeyText;
public string TargetTargetingViewHotkeyText
{
    get => _targetTargetingViewHotkeyText;
    set => SetProperty(ref _targetTargetingViewHotkeyText, value);
}

    #endregion

    #region Public Methods

    public void StartSettingHotkey(string actionId)
    {
        if (_currentSettingHotkeyId != null &&
            _propertySetters.TryGetValue(_currentSettingHotkeyId, out var prevSetter))
        {
            prevSetter(GetHotkeyDisplayText(_currentSettingHotkeyId));
        }

        _currentSettingHotkeyId = actionId;

        if (_propertySetters.TryGetValue(actionId, out var setter))
        {
            setter("Press keys...");
        }

        _tempHook = new LowLevelKeyboardHook();
        _tempHook.IsExtendedMode = true;
        _tempHook.Down += TempHook_Down;
        _tempHook.Start();
    }
    
    public void ConfirmHotkey()
    {
            
        var currentSettingHotkeyId = _currentSettingHotkeyId;
        var currentKeys = _currentKeys;
        if (currentSettingHotkeyId == null || currentKeys == null || currentKeys.IsEmpty)
        {
            CancelSettingHotkey();
            return;
        }

        HandleExistingHotkey(currentKeys);
        SetNewHotkey(currentSettingHotkeyId, currentKeys);

        StopSettingHotkey();
    }
    
    public void CancelSettingHotkey()
    {
        if (_currentSettingHotkeyId != null &&
            _propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
        {
            setter("None");
            _hotkeyManager.SetHotkey(_currentSettingHotkeyId, new Keys());
        }

        StopSettingHotkey();
    }

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.Quitout.ToString(), () => _settingsService.Quitout());
    }
    
    private void LoadHotkeyDisplays()
    {
        foreach (var entry in _propertySetters)
        {
            string actionId = entry.Key;
            Action<string> setter = entry.Value;

            setter(GetHotkeyDisplayText(actionId));
        }
    }
        
    private string GetHotkeyDisplayText(string actionId)
    {
        Keys keys = _hotkeyManager.GetHotkey(actionId);
        return keys != null && keys.Values.ToArray().Length > 0 ? string.Join(" + ", keys) : "None";
    }
    
    private void TempHook_Down(object sender, KeyboardEventArgs e)
    {
        if (_currentSettingHotkeyId == null || e.Keys.IsEmpty)
            return;

        try
        {
            bool containsEnter = e.Keys.Values.Contains(Key.Enter) || e.Keys.Values.Contains(Key.Return);

            if (containsEnter && _currentKeys != null)
            {
                _hotkeyManager.SetHotkey(_currentSettingHotkeyId, _currentKeys);
                StopSettingHotkey();
                e.IsHandled = true;
                return;
            }

            if (e.Keys.Values.Contains(Key.Escape))
            {
                CancelSettingHotkey();
                e.IsHandled = true;
                return;
            }

            if (containsEnter)
            {
                e.IsHandled = true;
                return;
            }

            if (e.Keys.IsEmpty)
                return;

            _currentKeys = e.Keys;

            if (_propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
            {
                string keyText = e.Keys.ToString();
                setter(keyText);
            }
        }
        catch (Exception ex)
        {
            if (_propertySetters.TryGetValue(_currentSettingHotkeyId, out var setter))
            {
                setter("Error: Invalid key combination");
            }
        }

        e.IsHandled = true;
    }
    
    

    private void StopSettingHotkey()
    {
        if (_tempHook != null)
        {
            _tempHook.Down -= TempHook_Down;
            _tempHook.Dispose();
            _tempHook = null;
        }

        _currentSettingHotkeyId = null;
        _currentKeys = null;
    }
    
    private void HandleExistingHotkey(Keys currentKeys)
    {
        string existingHotkeyId = _hotkeyManager.GetActionIdByKeys(currentKeys);
        if (string.IsNullOrEmpty(existingHotkeyId)) return;
            
        _hotkeyManager.ClearHotkey(existingHotkeyId);
        if (_propertySetters.TryGetValue(existingHotkeyId, out var oldSetter))
        {
            oldSetter("None");
        }
    }
        
    private void SetNewHotkey(string currentSettingHotkeyId, Keys currentKeys)
    {
        _hotkeyManager.SetHotkey(currentSettingHotkeyId, currentKeys);
            
        if (_propertySetters.TryGetValue(currentSettingHotkeyId, out var setter))
        {
            setter(new Keys(currentKeys.Values.ToArray()).ToString());
        }
    }

    #endregion
    
    
}