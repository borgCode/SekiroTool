using System.Windows;
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

    public SettingsViewModel(ISettingsService settingsService, IStateService stateService,
        HotkeyManager hotkeyManager)
    {
        _settingsService = settingsService;
        _hotkeyManager = hotkeyManager;

        stateService.Subscribe(State.Attached, OnGameAttached);
        stateService.Subscribe(State.EarlyAttached, OnGameEarlyAttached);
        
        RegisterHotkeys();

        _propertySetters = new Dictionary<string, Action<string>>
        {
            { HotkeyActions.SavePos1.ToString(), text => SavePos1HotkeyText = text },
            { HotkeyActions.SavePos2.ToString(), text => SavePos2HotkeyText = text },
            { HotkeyActions.RestorePos1.ToString(), text => RestorePos1HotkeyText = text },
            { HotkeyActions.RestorePos2.ToString(), text => RestorePos2HotkeyText = text },
            { HotkeyActions.ApplyConfetti.ToString(), text => ApplyConfettiHotkeyText = text },
            { HotkeyActions.ApplyGachiin.ToString(), text => ApplyGachiinHotkeyText = text },
            { HotkeyActions.RemoveConfetti.ToString(), text => RemoveConfettiHotkeyText = text },
            { HotkeyActions.RemoveGachiin.ToString(), text => RemoveGachiinHotkeyText = text },
            
            { HotkeyActions.OneShotHealth.ToString(), text => OneShotHealthHotkeyText = text },
            { HotkeyActions.OneShotPosture.ToString(), text => OneShotPostureHotkeyText = text },
            { HotkeyActions.NoGoodsConsume.ToString(), text => NoGoodsConsumeHotkeyText = text },
            { HotkeyActions.NoEmblemConsume.ToString(), text => NoEmblemConsumeHotkeyText = text },
            { HotkeyActions.InfiniteRevival.ToString(), text => InfiniteRevivalHotkeyText = text },
            { HotkeyActions.PlayerHide.ToString(), text => PlayerHideHotkeyText = text },
            { HotkeyActions.PlayerSilent.ToString(), text => PlayerSilentHotkeyText = text },
            { HotkeyActions.InfinitePoise.ToString(), text => InfinitePoiseHotkeyText = text },
            { HotkeyActions.NoDeath.ToString(), text => NoDeathHotkeyText = text },
            { HotkeyActions.NoDeathExKillbox.ToString(), text => NoDeathExKillboxHotkeyText = text },
            
            { HotkeyActions.IncreasePlayerSpeed.ToString(), text => IncreasePlayerSpeedHotkeyText = text },
            { HotkeyActions.DecreasePlayerSpeed.ToString(), text => DecreasePlayerSpeedHotkeyText = text },
            { HotkeyActions.TogglePlayerSpeed.ToString(), text => TogglePlayerSpeedHotkeyText = text },
            { HotkeyActions.NoDamage.ToString(), text => TogglePlayerNoDamageHotkeyText = text},
            // { "RTSR", text => RtsrHotkeyText = text },
            // { "NoDeath", text => NoDeathHotkeyText = text },
            // { "OneShot", text => OneShotHotkeyText = text },
            // { "PlayerNoDamage", text => NoDamagePlayerHotkeyText = text },
            // { "TogglePlayerSpeed", text => TogglePlayerSpeedHotkeyText = text },
            // { "IncreasePlayerSpeed", text => IncreasePlayerSpeedHotkeyText = text },
            // { "DecreasePlayerSpeed", text => DecreasePlayerSpeedHotkeyText = text },
            // { "DealNoDamage", text => DealNoDamageHotkeyText = text },
            { HotkeyActions.SkipDragonPhaseOne.ToString(), text => SkipDragonPhaseOneHotkeyText = text },
            { HotkeyActions.TriggerDragonFinalAttack.ToString(), text => TriggerDragonFinalAttackHotkeyText = text },
            { HotkeyActions.NoButterflySummons.ToString(), text => NoButterflySummonsHotkeyText = text },
            { HotkeyActions.AllNoDeath.ToString(), text => AllNoDeathHotkeyText = text },
            { HotkeyActions.AllNoDamage.ToString(), text => AllNoDamageHotkeyText = text },
            { HotkeyActions.AllNoHit.ToString(), text => AllNoHitHotkeyText = text },
            { HotkeyActions.AllNoAttack.ToString(), text => AllNoAttackHotkeyText = text },
            { HotkeyActions.AllNoMove.ToString(), text => AllNoMoveHotkeyText = text },
            { HotkeyActions.AllDisableAi.ToString(), text => AllDisableAiHotkeyText = text },
            { HotkeyActions.AllNoPostureBuildup.ToString(), text => AllNoPostureBuildupHotkeyText = text },
            { HotkeyActions.AllTargetingView.ToString(), text => AllTargetingViewHotkeyText = text },
            // { "RestoreSpellcasts", text => RestoreSpellcastsHotkeyText = text },
            // { "RestoreHumanity", text => RestoreHumanityHotkeyText = text },
            // { "Rest", text => RestHotkeyText = text },

            { HotkeyActions.Quitout.ToString(), text => QuitoutHotkeyText = text },
            // { "Warp", text => WarpHotkeyText = text },

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
            { HotkeyActions.IncrementForceAct.ToString(), text => IncrementForceActHotkeyText = text },
            { HotkeyActions.DecrementForceAct.ToString(), text => DecrementForceActHotkeyText = text },
            { HotkeyActions.IncrementForceKengekiAct.ToString(), text => IncrementForceKengekiActHotkeyText = text },
            { HotkeyActions.DecrementForceKengekiAct.ToString(), text => DecrementForceKengekiActHotkeyText = text },
            { HotkeyActions.IncreaseTargetSpeed.ToString(), text => IncreaseTargetSpeedHotkeyText = text },
            { HotkeyActions.DecreaseTargetSpeed.ToString(), text => DecreaseTargetSpeedHotkeyText = text },
            { HotkeyActions.ToggleTargetSpeed.ToString(), text => ToggleTargetSpeedHotkeyText = text },
            { HotkeyActions.FreezeTargetAi.ToString(), text => FreezeTargetAiHotkeyText = text },
            { HotkeyActions.NoAttackTargetAi.ToString(), text => NoAttackTargetAiHotkeyText = text },
            { HotkeyActions.NoMoveTargetAi.ToString(), text => NoMoveTargetAiHotkeyText = text },
            { HotkeyActions.TargetNoPostureBuildup.ToString(), text => TargetNoPostureBuildupHotkeyText = text },
            { HotkeyActions.TargetNoDeath.ToString(), text => TargetNoDeathHotkeyText = text },
            { HotkeyActions.TargetTargetingView.ToString(), text => TargetTargetingViewHotkeyText = text },
            { HotkeyActions.ToggleGameSpeed.ToString(), text => ToggleGameSpeedHotkeyText = text },
            { HotkeyActions.IncreaseGameSpeed.ToString(), text => IncreaseGameSpeedHotkeyText = text },
            { HotkeyActions.DecreaseGameSpeed.ToString(), text => DecreaseGameSpeedHotkeyText = text },
            { HotkeyActions.NoClip.ToString(), text => NoClipHotkeyText = text },
            { HotkeyActions.IncreaseNoClipSpeed.ToString(), text => IncreaseNoClipSpeedHotkeyText = text },
            { HotkeyActions.DecreaseNoClipSpeed.ToString(), text => DecreaseNoClipSpeedHotkeyText = text },
            { HotkeyActions.FreeCam.ToString(), text => FreeCamHotkeyText = text },
            { HotkeyActions.MoveCamToPlayer.ToString(), text => MoveCamToPlayerHotkeyText = text },
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

    private bool _isSaveInCombatEnabled;

    public bool IsSaveInCombatEnabled
    {
        get => _isSaveInCombatEnabled;
        set
        {
            if (SetProperty(ref _isSaveInCombatEnabled, value))
            {
                SettingsManager.Default.SaveInCombat = value;
                SettingsManager.Default.Save();

                _settingsService.ToggleSaveInCombat(_isSaveInCombatEnabled);
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
    
    private string _increasePlayerSpeedHotkeyText;
    
    public string IncreasePlayerSpeedHotkeyText
    {
        get => _increasePlayerSpeedHotkeyText;
        set => SetProperty(ref _increasePlayerSpeedHotkeyText, value);
    }
    
    private string _decreasePlayerSpeedHotkeyText;
    
    public string DecreasePlayerSpeedHotkeyText
    {
        get => _decreasePlayerSpeedHotkeyText;
        set => SetProperty(ref _decreasePlayerSpeedHotkeyText, value);
    }
    
    private string _togglePlayerSpeedHotkeyText;
    
    public string TogglePlayerSpeedHotkeyText
    {
        get => _togglePlayerSpeedHotkeyText;
        set => SetProperty(ref _togglePlayerSpeedHotkeyText, value);
    }
    
    private string _togglePlayerNoDamageHotkeyText;
    
    public string TogglePlayerNoDamageHotkeyText
    {
        get => _togglePlayerNoDamageHotkeyText;
        set => SetProperty(ref _togglePlayerNoDamageHotkeyText, value);
    }

    private string _savePos1HotkeyText;

    public string SavePos1HotkeyText
    {
        get => _savePos1HotkeyText;
        set => SetProperty(ref _savePos1HotkeyText, value);
    }

    private string _savePos2HotkeyText;

    public string SavePos2HotkeyText
    {
        get => _savePos2HotkeyText;
        set => SetProperty(ref _savePos2HotkeyText, value);
    }

    private string _restorePos1HotkeyText;

    public string RestorePos1HotkeyText
    {
        get => _restorePos1HotkeyText;
        set => SetProperty(ref _restorePos1HotkeyText, value);
    }

    private string _restorePos2HotkeyText;

    public string RestorePos2HotkeyText
    {
        get => _restorePos2HotkeyText;
        set => SetProperty(ref _restorePos2HotkeyText, value);
    }
    
    private string _oneShotHealthHotkeyText;

    public string OneShotHealthHotkeyText
    {
        get => _oneShotHealthHotkeyText;
        set => SetProperty(ref _oneShotHealthHotkeyText, value);
    }

    private string _oneShotPostureHotkeyText;

    public string OneShotPostureHotkeyText
    {
        get => _oneShotPostureHotkeyText;
        set => SetProperty(ref _oneShotPostureHotkeyText, value);
    }

    private string _noGoodsConsumeHotkeyText;

    public string NoGoodsConsumeHotkeyText
    {
        get => _noGoodsConsumeHotkeyText;
        set => SetProperty(ref _noGoodsConsumeHotkeyText, value);
    }

    private string _noEmblemConsumeHotkeyText;

    public string NoEmblemConsumeHotkeyText
    {
        get => _noEmblemConsumeHotkeyText;
        set => SetProperty(ref _noEmblemConsumeHotkeyText, value);
    }

    private string _infiniteRevivalHotkeyText;

    public string InfiniteRevivalHotkeyText
    {
        get => _infiniteRevivalHotkeyText;
        set => SetProperty(ref _infiniteRevivalHotkeyText, value);
    }

    private string _playerHideHotkeyText;

    public string PlayerHideHotkeyText
    {
        get => _playerHideHotkeyText;
        set => SetProperty(ref _playerHideHotkeyText, value);
    }

    private string _playerSilentHotkeyText;

    public string PlayerSilentHotkeyText
    {
        get => _playerSilentHotkeyText;
        set => SetProperty(ref _playerSilentHotkeyText, value);
    }

    private string _infinitePoiseHotkeyText;

    public string InfinitePoiseHotkeyText
    {
        get => _infinitePoiseHotkeyText;
        set => SetProperty(ref _infinitePoiseHotkeyText, value);
    }
    

    private string _noDeathHotkeyText;

    public string NoDeathHotkeyText
    {
        get => _noDeathHotkeyText;
        set => SetProperty(ref _noDeathHotkeyText, value);
    }

    private string _noDeathExKillboxHotkeyText;

    public string NoDeathExKillboxHotkeyText
    {
        get => _noDeathExKillboxHotkeyText;
        set => SetProperty(ref _noDeathExKillboxHotkeyText, value);
    }

    private string _applyConfettiHotkeyText;
    
    public string ApplyConfettiHotkeyText
    {
        get => _applyConfettiHotkeyText;
        set => SetProperty(ref _applyConfettiHotkeyText, value);
    }

    private string _applyGachiinHotkeyText;
    
    public string ApplyGachiinHotkeyText
    {
        get => _applyGachiinHotkeyText;
        set => SetProperty(ref _applyGachiinHotkeyText, value);
    }
    
    private string _removeConfettiHotkeyText;

    public string RemoveConfettiHotkeyText
    {
        get => _removeConfettiHotkeyText;
        set => SetProperty(ref _removeConfettiHotkeyText, value);
    }
    
    private string _removeGachiinHotkeyText;

    public string RemoveGachiinHotkeyText
    {
        get => _removeGachiinHotkeyText;
        set => SetProperty(ref _removeGachiinHotkeyText, value);
    }

    private string _skipDragonPhaseOneHotkeyText;

    public string SkipDragonPhaseOneHotkeyText
    {
        get => _skipDragonPhaseOneHotkeyText;
        set => SetProperty(ref _skipDragonPhaseOneHotkeyText, value);
    }
    
    private string _triggerDragonFinalAttackHotkeyText;

    public string TriggerDragonFinalAttackHotkeyText
    {
        get => _triggerDragonFinalAttackHotkeyText;
        set => SetProperty(ref _triggerDragonFinalAttackHotkeyText, value);
    }
    
    private string _noButterflySummonsHotkeyText;
    public string NoButterflySummonsHotkeyText
    {
        get => _noButterflySummonsHotkeyText;
        set => SetProperty(ref _noButterflySummonsHotkeyText, value);
    }

    private string _allNoDeathHotkeyText;
    public string AllNoDeathHotkeyText
    {
        get => _allNoDeathHotkeyText;
        set => SetProperty(ref _allNoDeathHotkeyText, value);
    }

    private string _allNoDamageHotkeyText;
    public string AllNoDamageHotkeyText
    {
        get => _allNoDamageHotkeyText;
        set => SetProperty(ref _allNoDamageHotkeyText, value);
    }

    private string _allNoHitHotkeyText;
    public string AllNoHitHotkeyText
    {
        get => _allNoHitHotkeyText;
        set => SetProperty(ref _allNoHitHotkeyText, value);
    }

    private string _allNoAttackHotkeyText;
    public string AllNoAttackHotkeyText
    {
        get => _allNoAttackHotkeyText;
        set => SetProperty(ref _allNoAttackHotkeyText, value);
    }

    private string _allNoMoveHotkeyText;
    public string AllNoMoveHotkeyText
    {
        get => _allNoMoveHotkeyText;
        set => SetProperty(ref _allNoMoveHotkeyText, value);
    }

    private string _allDisableAiHotkeyText;
    public string AllDisableAiHotkeyText
    {
        get => _allDisableAiHotkeyText;
        set => SetProperty(ref _allDisableAiHotkeyText, value);
    }

    private string _allNoPostureBuildupHotkeyText;
    public string AllNoPostureBuildupHotkeyText
    {
        get => _allNoPostureBuildupHotkeyText;
        set => SetProperty(ref _allNoPostureBuildupHotkeyText, value);
    }

    private string _allTargetingViewHotkeyText;
    public string AllTargetingViewHotkeyText
    {
        get => _allTargetingViewHotkeyText;
        set => SetProperty(ref _allTargetingViewHotkeyText, value);
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
    
    private string _incrementForceActHotkeyText;

    public string IncrementForceActHotkeyText
    {
        get => _incrementForceActHotkeyText;
        set => SetProperty(ref _incrementForceActHotkeyText, value);
    }

    private string _decrementForceActHotkeyText;

    public string DecrementForceActHotkeyText
    {
        get => _decrementForceActHotkeyText;
        set => SetProperty(ref _decrementForceActHotkeyText, value);
    }
    
    private string _incrementForceKengekiActHotkeyText;

    public string IncrementForceKengekiActHotkeyText
    {
        get => _incrementForceKengekiActHotkeyText;
        set => SetProperty(ref _incrementForceKengekiActHotkeyText, value);
    }

    private string _decrementForceKengekiActHotkeyText;

    public string DecrementForceKengekiActHotkeyText
    {
        get => _decrementForceKengekiActHotkeyText;
        set => SetProperty(ref _decrementForceKengekiActHotkeyText, value);
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


    private string _toggleGameSpeedHotkeyText;

    public string ToggleGameSpeedHotkeyText
    {
        get => _toggleGameSpeedHotkeyText;
        set => SetProperty(ref _toggleGameSpeedHotkeyText, value);
    }

    private string _increaseGameSpeedHotkeyText;

    public string IncreaseGameSpeedHotkeyText
    {
        get => _increaseGameSpeedHotkeyText;
        set => SetProperty(ref _increaseGameSpeedHotkeyText, value);
    }

    private string _decreaseGameSpeedHotkeyText;

    public string DecreaseGameSpeedHotkeyText
    {
        get => _decreaseGameSpeedHotkeyText;
        set => SetProperty(ref _decreaseGameSpeedHotkeyText, value);
    }
    
    

    private string _noClipHotkeyText;

    public string NoClipHotkeyText
    {
        get => _noClipHotkeyText;
        set => SetProperty(ref _noClipHotkeyText, value);
    }

    private string _increaseNoClipSpeedHotkeyText;

    public string IncreaseNoClipSpeedHotkeyText
    {
        get => _increaseNoClipSpeedHotkeyText;
        set => SetProperty(ref _increaseNoClipSpeedHotkeyText, value);
    }

    private string _decreaseNoClipSpeedHotkeyText;

    public string DecreaseNoClipSpeedHotkeyText
    {
        get => _decreaseNoClipSpeedHotkeyText;
        set => SetProperty(ref _decreaseNoClipSpeedHotkeyText, value);
    }

    private string _freeCamHotkeyText;

    public string FreeCamHotkeyText
    {
        get => _freeCamHotkeyText;
        set => SetProperty(ref _freeCamHotkeyText, value);
    }

    private string _moveCamToPlayerHotkeyText;

    public string MoveCamToPlayerHotkeyText
    {
        get => _moveCamToPlayerHotkeyText;
        set => SetProperty(ref _moveCamToPlayerHotkeyText, value);
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

        _isSaveInCombatEnabled = SettingsManager.Default.SaveInCombat;
        OnPropertyChanged(nameof(IsSaveInCombatEnabled));

        _isNoCameraSpinEnabled = SettingsManager.Default.NoCameraSpin;
        OnPropertyChanged(nameof(IsNoCameraSpinEnabled));
        
        _isDisableMenuMusicEnabled = SettingsManager.Default.DisableMenuMusic;
        OnPropertyChanged(nameof(IsDisableMenuMusicEnabled));
            
        _isDefaultSoundChangeEnabled = SettingsManager.Default.DefaultSoundChangeEnabled;
        OnPropertyChanged(nameof(IsDefaultSoundChangeEnabled));

        _defaultSoundVolume = SettingsManager.Default.DefaultSoundVolume;
        OnPropertyChanged(nameof(DefaultSoundVolume));

        IsAlwaysOnTopEnabled = SettingsManager.Default.AlwaysOnTop;
    }

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.Quitout.ToString(), () => _settingsService.Quitout());
    }

    private void OnGameAttached()
    {
        if (IsNoTutorialsEnabled) _settingsService.ToggleNoTutorials(true);
        if (IsSaveInCombatEnabled) _settingsService.ToggleSaveInCombat(true);
        if (IsNoCameraSpinEnabled) _settingsService.ToggleNoCameraSpin(true);
    }
    
    private void OnGameEarlyAttached()
    {
        if (IsNoLogoEnabled) _settingsService.ToggleNoLogo(true);
        if (IsDefaultSoundChangeEnabled) _settingsService.PatchDefaultSound(DefaultSoundVolume);
        if (IsDisableMenuMusicEnabled) _settingsService.ToggleDisableMusic(true);
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