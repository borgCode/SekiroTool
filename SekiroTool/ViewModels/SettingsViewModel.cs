using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using H.Hooks;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using Key = H.Hooks.Key;
using KeyboardEventArgs = H.Hooks.KeyboardEventArgs;

namespace SekiroTool.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settingsService;
    private readonly HotkeyManager _hotkeyManager;

    private readonly Dictionary<string, HotkeyBindingViewModel> _hotkeyLookup;

    private string _currentSettingHotkeyId;
    private LowLevelKeyboardHook _tempHook;
    private Keys _currentKeys;

    public ObservableCollection<HotkeyBindingViewModel> PlayerHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> EnemiesHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> TargetHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> UtilityHotkeys { get; }
    public ObservableCollection<HotkeyBindingViewModel> EventHotkeys { get; }

    public SettingsViewModel(ISettingsService settingsService, IStateService stateService,
        HotkeyManager hotkeyManager)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        stateService.Subscribe(State.Attached, OnGameAttached);
        stateService.Subscribe(State.EarlyAttached, OnGameEarlyAttached);
        stateService.Subscribe(State.GameStart, OnGameStart);
        stateService.Subscribe(State.Loaded, OnGameLoaded);

        RegisterHotkeys();

        PlayerHotkeys =
        [
            new("Save Position 1", HotkeyActions.SavePos1),
            new("Save Position 2", HotkeyActions.SavePos2),
            new("Restore Position 1", HotkeyActions.RestorePos1),
            new("Restore Position 2", HotkeyActions.RestorePos2),
            new("Apply Confetti", HotkeyActions.ApplyConfetti),
            new("Apply Gachiin", HotkeyActions.ApplyGachiin),
            new("Remove Confetti", HotkeyActions.RemoveConfetti),
            new("Remove Gachiin", HotkeyActions.RemoveGachiin),
            new("One Shot Health", HotkeyActions.OneShotHealth),
            new("One Shot Posture", HotkeyActions.OneShotPosture),
            new("No Goods Consume", HotkeyActions.NoGoodsConsume),
            new("No Emblem Consume", HotkeyActions.NoEmblemConsume),
            new("Infinite Revival", HotkeyActions.InfiniteRevival),
            new("Player Hide", HotkeyActions.PlayerHide),
            new("Player Silent", HotkeyActions.PlayerSilent),
            new("Infinite Poise", HotkeyActions.InfinitePoise),
            new("No Death", HotkeyActions.NoDeath),
            new("No Death (Yes Killbox)", HotkeyActions.NoDeathExKillbox),
            new("Increase Player Speed", HotkeyActions.IncreasePlayerSpeed),
            new("Decrease Player Speed", HotkeyActions.DecreasePlayerSpeed),
            new("Toggle Player Speed", HotkeyActions.TogglePlayerSpeed),
            new("No Damage", HotkeyActions.NoDamage),
            new("Increase Damage Multiplier", HotkeyActions.IncreaseDamageMultiplier),
            new("Decrease Damage Multiplier", HotkeyActions.DecreaseDamageMultiplier),
            new("Toggle Damage Multiplier", HotkeyActions.ToggleDamageMultiplier),
        ];

        EnemiesHotkeys =
        [
            new("Skip Dragon Phase One", HotkeyActions.SkipDragonPhaseOne),
            new("Trigger Dragon Final Attack", HotkeyActions.TriggerDragonFinalAttack),
            new("No Butterfly Summons", HotkeyActions.NoButterflySummons),
            new("Snake Intro Loop", HotkeyActions.SnakeIntroLoop),
            new("All No Death", HotkeyActions.AllNoDeath),
            new("All No Damage", HotkeyActions.AllNoDamage),
            new("All No Hit", HotkeyActions.AllNoHit),
            new("All No Attack", HotkeyActions.AllNoAttack),
            new("All No Move", HotkeyActions.AllNoMove),
            new("All Disable AI", HotkeyActions.AllDisableAi),
            new("All No Posture Buildup", HotkeyActions.AllNoPostureBuildup),
            new("All Targeting View", HotkeyActions.AllTargetingView),
        ];

        UtilityHotkeys =
        [
            new("Quitout", HotkeyActions.Quitout),
            new("Toggle Game Speed", HotkeyActions.ToggleGameSpeed),
            new("Increase Game Speed", HotkeyActions.IncreaseGameSpeed),
            new("Decrease Game Speed", HotkeyActions.DecreaseGameSpeed),
            new("No Clip", HotkeyActions.NoClip),
            new("Increase No Clip Speed", HotkeyActions.IncreaseNoClipSpeed),
            new("Decrease No Clip Speed", HotkeyActions.DecreaseNoClipSpeed),
            new("Free Cam", HotkeyActions.FreeCam),
            new("Move Cam To Player", HotkeyActions.MoveCamToPlayer),
        ];
        
        EventHotkeys =
        [
            new("Skip Emma", HotkeyActions.EmmaSkip)
        ];

        TargetHotkeys =
        [
            new("Enable Target Options", HotkeyActions.EnableTargetOptions),
            new("Freeze Target HP", HotkeyActions.FreezeTargetHp),
            new("Set Target One HP", HotkeyActions.SetTargetOneHp),
            new("Target Custom HP", HotkeyActions.TargetCustomHp),
            new("Freeze Target Posture", HotkeyActions.FreezeTargetPosture),
            new("Set Target One Posture", HotkeyActions.SetTargetOnePosture),
            new("Target Custom Posture", HotkeyActions.TargetCustomPosture),
            new("Show All Resistances", HotkeyActions.ShowAllResistances),
            new("Repeat Act", HotkeyActions.RepeatAct),
            new("Repeat Kengeki Act", HotkeyActions.RepeatKengekiAct),
            new("Increment Force Act", HotkeyActions.IncrementForceAct),
            new("Decrement Force Act", HotkeyActions.DecrementForceAct),
            new("Increment Force Kengeki Act", HotkeyActions.IncrementForceKengekiAct),
            new("Decrement Force Kengeki Act", HotkeyActions.DecrementForceKengekiAct),
            new("Increase Target Speed", HotkeyActions.IncreaseTargetSpeed),
            new("Decrease Target Speed", HotkeyActions.DecreaseTargetSpeed),
            new("Toggle Target Speed", HotkeyActions.ToggleTargetSpeed),
            new("Freeze Target AI", HotkeyActions.FreezeTargetAi),
            new("No Attack Target AI", HotkeyActions.NoAttackTargetAi),
            new("No Move Target AI", HotkeyActions.NoMoveTargetAi),
            new("Target No Posture Buildup", HotkeyActions.TargetNoPostureBuildup),
            new("Target No Death", HotkeyActions.TargetNoDeath),
            new("Target Targeting View", HotkeyActions.TargetTargetingView),
        ];
        
        _hotkeyLookup = PlayerHotkeys
            .Concat(EnemiesHotkeys)
            .Concat(TargetHotkeys)
            .Concat(UtilityHotkeys)
            .Concat((EventHotkeys))
            .ToDictionary(h => h.ActionId);

        LoadHotkeyDisplays();
        
        ClearHotkeysCommand = new DelegateCommand(ClearHotkeys);
    }

  

    #region Commands

    public ICommand ClearHotkeysCommand { get; set; }

    #endregion

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

    private bool _isNoLogoEnabled;

    public bool IsNoLogoEnabled
    {
        get => _isNoLogoEnabled;
        set
        {
            if (SetProperty(ref _isNoLogoEnabled, value))
            {
                SettingsManager.Default.NoLogo = value;
                SettingsManager.Default.Save();

                _settingsService.ToggleNoLogo(_isNoLogoEnabled);
            }
        }
    }

    private bool _isAlwaysOnTopEnabled;

    public bool IsAlwaysOnTopEnabled
    {
        get => _isAlwaysOnTopEnabled;
        set
        {
            if (!SetProperty(ref _isAlwaysOnTopEnabled, value)) return;
            SettingsManager.Default.AlwaysOnTop = value;
            SettingsManager.Default.Save();
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null) mainWindow.Topmost = _isAlwaysOnTopEnabled;
        }
    }

    private bool _isNoTutorialsEnabled;

    public bool IsNoTutorialsEnabled
    {
        get => _isNoTutorialsEnabled;
        set
        {
            if (SetProperty(ref _isNoTutorialsEnabled, value))
            {
                SettingsManager.Default.NoTutorials = value;
                SettingsManager.Default.Save();

                _settingsService.ToggleNoTutorials(_isNoTutorialsEnabled);
            }
        }
    }

    private bool _isHotkeyReminderEnabled;

    public bool IsHotkeyReminderEnabled
    {
        get => _isHotkeyReminderEnabled;
        set
        {
            if (SetProperty(ref _isHotkeyReminderEnabled, value))
            {
                SettingsManager.Default.HotkeyReminder = value;
                SettingsManager.Default.Save();
            }
        }
    }

    private bool _isNoCameraSpinEnabled;

    public bool IsNoCameraSpinEnabled
    {
        get => _isNoCameraSpinEnabled;
        set
        {
            if (SetProperty(ref _isNoCameraSpinEnabled, value))
            {
                SettingsManager.Default.NoCameraSpin = value;
                SettingsManager.Default.Save();

                _settingsService.ToggleNoCameraSpin(_isNoCameraSpinEnabled);
            }
        }
    }

    private bool _isDisableMenuMusicEnabled;

    public bool IsDisableMenuMusicEnabled
    {
        get => _isDisableMenuMusicEnabled;
        set
        {
            if (!SetProperty(ref _isDisableMenuMusicEnabled, value)) return;
            SettingsManager.Default.DisableMenuMusic = value;
            SettingsManager.Default.Save();
            if (_isDisableMenuMusicEnabled)
            {
                _settingsService.StopMusic();
            }
            _settingsService.ToggleDisableMusic(_isDisableMenuMusicEnabled);
            
        }
    }

    private bool _isDefaultSoundChangeEnabled;

    public bool IsDefaultSoundChangeEnabled
    {
        get => _isDefaultSoundChangeEnabled;
        set
        {
            if (!SetProperty(ref _isDefaultSoundChangeEnabled, value)) return;
            SettingsManager.Default.DefaultSoundChangeEnabled = value;
            SettingsManager.Default.Save();
        }
    }

    private int _defaultSoundVolume;

    public int DefaultSoundVolume
    {
        get => _defaultSoundVolume;
        set
        {
            if (!SetProperty(ref _defaultSoundVolume, value)) return;
            if (!IsDefaultSoundChangeEnabled) return;
            SettingsManager.Default.DefaultSoundVolume = value;
            SettingsManager.Default.Save();
        }
    }

    public bool _isNoCutsceneEnabled;

    public bool IsNoCutsceneEnabled
    {
        get => _isNoCutsceneEnabled;
        set
        {
            if (SetProperty(ref _isNoCutsceneEnabled, value))
            {
                SettingsManager.Default.DisableCutscenes = value;
                SettingsManager.Default.Save();

                _settingsService.ToggleDisableCutscenes(_isNoCutsceneEnabled);
            }
        }
    }

    #endregion

    #region Public Methods

    public void StartSettingHotkey(string actionId)
    {
        if (_currentSettingHotkeyId != null &&
            _hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var prev))
        {
            prev.HotkeyText = GetHotkeyDisplayText(_currentSettingHotkeyId);
        }

        _currentSettingHotkeyId = actionId;

        if (_hotkeyLookup.TryGetValue(actionId, out var current))
        {
            current.HotkeyText = "Press keys...";
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
        var actionId = _currentSettingHotkeyId;

        if (actionId != null && _hotkeyLookup.TryGetValue(actionId, out var binding))
        {
            binding.HotkeyText = "None";
            _hotkeyManager.SetHotkey(actionId, new Keys());
        }

        StopSettingHotkey();
    }

    public void ApplyStartUpOptions()
    {
        _isEnableHotkeysEnabled = SettingsManager.Default.EnableHotkeys;
        if (_isEnableHotkeysEnabled) _hotkeyManager.Start();
        else _hotkeyManager.Stop();
        OnPropertyChanged(nameof(IsEnableHotkeysEnabled));

        _isNoLogoEnabled = SettingsManager.Default.NoLogo;
        OnPropertyChanged(nameof(IsNoLogoEnabled));

        _isNoTutorialsEnabled = SettingsManager.Default.NoTutorials;
        OnPropertyChanged(nameof(IsNoTutorialsEnabled));

        _isNoCameraSpinEnabled = SettingsManager.Default.NoCameraSpin;
        OnPropertyChanged(nameof(IsNoCameraSpinEnabled));

        _isNoCutsceneEnabled = SettingsManager.Default.DisableCutscenes;
        OnPropertyChanged(nameof(IsNoCutsceneEnabled));

        _isDisableMenuMusicEnabled = SettingsManager.Default.DisableMenuMusic;
        OnPropertyChanged(nameof(IsDisableMenuMusicEnabled));

        _isDefaultSoundChangeEnabled = SettingsManager.Default.DefaultSoundChangeEnabled;
        OnPropertyChanged(nameof(IsDefaultSoundChangeEnabled));

        _defaultSoundVolume = SettingsManager.Default.DefaultSoundVolume;
        OnPropertyChanged(nameof(DefaultSoundVolume));

        _isHotkeyReminderEnabled = SettingsManager.Default.HotkeyReminder;
        OnPropertyChanged(nameof(IsHotkeyReminderEnabled));

        IsAlwaysOnTopEnabled = SettingsManager.Default.AlwaysOnTop;
    }

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.Quitout, () => _settingsService.Quitout());
    }
    
    private void OnGameLoaded()
    {
        if (IsNoTutorialsEnabled) _settingsService.ToggleNoTutorials(true);
        if (IsNoCameraSpinEnabled) _settingsService.ToggleNoCameraSpin(true);
    }

    private void OnGameAttached()
    {
      
        if (IsDefaultSoundChangeEnabled) _settingsService.PatchDefaultSound(DefaultSoundVolume);
        
        
        if (IsNoLogoEnabled) _settingsService.ToggleNoLogo(true);
        if (IsDisableMenuMusicEnabled) _settingsService.ToggleDisableMusic(true);
    }

    private void OnGameEarlyAttached()
    {
        if (IsNoLogoEnabled) _settingsService.ToggleNoLogo(true);
        if (IsDefaultSoundChangeEnabled) _settingsService.PatchDefaultSound(DefaultSoundVolume);
        if (IsDisableMenuMusicEnabled) _settingsService.ToggleDisableMusic(true);
    }

    private void OnGameStart()
    {
        if (!IsHotkeyReminderEnabled) return;
        if (!IsEnableHotkeysEnabled) return;
        MsgBox.Show("Hotkeys are enabled");
    }

    private void LoadHotkeyDisplays()
    {
        foreach (var hotkey in _hotkeyLookup.Values)
        {
            hotkey.HotkeyText = GetHotkeyDisplayText(hotkey.ActionId);
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

            if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
            {
                binding.HotkeyText = e.Keys.ToString();
            }
        }
        catch (Exception ex)
        {
            if (_hotkeyLookup.TryGetValue(_currentSettingHotkeyId, out var binding))
            {
                binding.HotkeyText = "Error: Invalid key combination";
            }
        }

        e.IsHandled = true;
    }

    private void StopSettingHotkey()
    {
        var hook = _tempHook;
        _tempHook = null;
        _currentSettingHotkeyId = null;
        _currentKeys = null;

        if (hook != null)
        {
            hook.Down -= TempHook_Down;
            try
            {
                hook.Dispose();
            }
            catch (COMException)
            {
                // Already stopped - harmless
            }
        }
    }

    private void HandleExistingHotkey(Keys currentKeys)
    {
        string existingHotkeyId = _hotkeyManager.GetActionIdByKeys(currentKeys);
        if (string.IsNullOrEmpty(existingHotkeyId)) return;

        _hotkeyManager.ClearHotkey(existingHotkeyId);
        if (_hotkeyLookup.TryGetValue(existingHotkeyId, out var binding))
        {
            binding.HotkeyText = "None";
        }
    }

    private void SetNewHotkey(string currentSettingHotkeyId, Keys currentKeys)
    {
        _hotkeyManager.SetHotkey(currentSettingHotkeyId, currentKeys);

        if (_hotkeyLookup.TryGetValue(currentSettingHotkeyId, out var binding))
        {
            binding.HotkeyText = new Keys(currentKeys.Values.ToArray()).ToString();
        }
    }
    
    private void ClearHotkeys()
    {
        _hotkeyManager.ClearAll();
        LoadHotkeyDisplays();
    }

    #endregion
}