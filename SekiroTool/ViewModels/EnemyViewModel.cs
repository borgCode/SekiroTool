using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyService _enemyService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IDebugDrawService _debugDrawService;
    private readonly IEventService _eventService;

    public EnemyViewModel(IEnemyService enemyService, HotkeyManager hotkeyManager, IGameStateService gameStateService,
        IDebugDrawService debugDrawService, IEventService eventService)
    {
        _enemyService = enemyService;
        _hotkeyManager = hotkeyManager;
        _debugDrawService = debugDrawService;
        _eventService = eventService;

        RegisterHotkeys();

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

        SkipDragonPhaseOneCommand = new DelegateCommand(SkipDragonPhaseOne);
        TriggerDragonFinalAttackCommand = new DelegateCommand(TriggerFinalDragonAttack);
    }

    
    #region Commands

    public ICommand SkipDragonPhaseOneCommand { get; set; }
    public ICommand TriggerDragonFinalAttackCommand { get; set; }
    
    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private bool _isNoDeathEnabled;

    public bool IsNoDeathEnabled
    {
        get => _isNoDeathEnabled;
        set
        {
            SetProperty(ref _isNoDeathEnabled, value);
            _enemyService.ToggleNoDeath(_isNoDeathEnabled);
        }
    }

    private bool _isNoDamageEnabled;

    public bool IsNoDamageEnabled
    {
        get => _isNoDamageEnabled;
        set
        {
            SetProperty(ref _isNoDamageEnabled, value);
            _enemyService.ToggleNoDamage(_isNoDamageEnabled);
        }
    }

    private bool _isNoHitEnabled;

    public bool IsNoHitEnabled
    {
        get => _isNoHitEnabled;
        set
        {
            SetProperty(ref _isNoHitEnabled, value);
            _enemyService.ToggleNoHit(_isNoHitEnabled);
        }
    }

    private bool _isNoAttackEnabled;

    public bool IsNoAttackEnabled
    {
        get => _isNoAttackEnabled;
        set
        {
            SetProperty(ref _isNoAttackEnabled, value);
            _enemyService.ToggleNoAttack(_isNoAttackEnabled);
        }
    }

    private bool _isNoMoveEnabled;

    public bool IsNoMoveEnabled
    {
        get => _isNoMoveEnabled;
        set
        {
            SetProperty(ref _isNoMoveEnabled, value);
            _enemyService.ToggleNoMove(_isNoMoveEnabled);
        }
    }

    private bool _isDisableAiEnabled;

    public bool IsDisableAiEnabled
    {
        get => _isDisableAiEnabled;
        set
        {
            SetProperty(ref _isDisableAiEnabled, value);
            _enemyService.ToggleDisableAi(_isDisableAiEnabled);
        }
    }

    private bool _isNoPostureBuildupEnabled;

    public bool IsNoPostureBuildupEnabled
    {
        get => _isNoPostureBuildupEnabled;
        set
        {
            SetProperty(ref _isNoPostureBuildupEnabled, value);
            _enemyService.ToggleNoPostureBuildup(_isNoPostureBuildupEnabled);
        }
    }

    private bool _isTargetingViewEnabled;

    public bool IsTargetingViewEnabled
    {
        get => _isTargetingViewEnabled;
        set
        {
            SetProperty(ref _isTargetingViewEnabled, value);
            if (_isTargetingViewEnabled) _debugDrawService.RequestDebugDraw();
            else _debugDrawService.ReleaseDebugDraw();
            _enemyService.ToggleTargetingView(_isTargetingViewEnabled);
        }
    }
    
    private bool _isDragonCombo1Enabled;

    public bool IsDragonCombo1Enabled
    {
        get => _isDragonCombo1Enabled;
        set
        {
            SetProperty(ref _isDragonCombo1Enabled, value);
            if (_isDragonCombo1Enabled)
            {
                IsDragonCombo2Enabled = false;
                IsDragonCombo3Enabled = false;
            }
            _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo1, _isDragonCombo1Enabled);
        }
    }
    
    private bool _isDragonCombo2Enabled;

    public bool IsDragonCombo2Enabled
    {
        get => _isDragonCombo2Enabled;
        set
        {
            SetProperty(ref _isDragonCombo2Enabled, value);
            if (_isDragonCombo2Enabled)
            {
                IsDragonCombo1Enabled = false;
                IsDragonCombo3Enabled = false;
            }
            _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo2, _isDragonCombo2Enabled);
        }
    }
    
    private bool _isDragonCombo3Enabled;

    public bool IsDragonCombo3Enabled
    {
        get => _isDragonCombo3Enabled;
        set
        {
            SetProperty(ref _isDragonCombo3Enabled, value);
            if (_isDragonCombo3Enabled)
            {
                IsDragonCombo1Enabled = false;
                IsDragonCombo2Enabled = false;
            }
            _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo3, _isDragonCombo3Enabled);
        }
    }

    private bool _isNoButterflySummonsEnabled;

    public bool IsNoButterflySummonsEnabled
    {
        get => _isNoButterflySummonsEnabled;
        set
        {
            SetProperty(ref _isNoButterflySummonsEnabled, value);
            _enemyService.ToggleButterflyNoSummons(_isNoButterflySummonsEnabled);
        }
    }

    #endregion


    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.SkipDragonPhaseOne.ToString(), () =>
        {
            if (!AreOptionsEnabled) return;
            _enemyService.SkipDragonPhaseOne();
        });
        _hotkeyManager.RegisterAction(HotkeyActions.TriggerDragonFinalAttack.ToString(), () =>
        {
            if (!AreOptionsEnabled) return;
            TriggerFinalDragonAttack();
        });
        
        _hotkeyManager.RegisterAction(HotkeyActions.NoButterflySummons.ToString(),
            () => { IsNoButterflySummonsEnabled = !IsNoButterflySummonsEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDeath.ToString(), () => { IsNoDeathEnabled = !IsNoDeathEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoDamage.ToString(), () => { IsNoDamageEnabled = !IsNoDamageEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoHit.ToString(), () => { IsNoHitEnabled = !IsNoHitEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoAttack.ToString(), () => { IsNoAttackEnabled = !IsNoAttackEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoMove.ToString(), () => { IsNoMoveEnabled = !IsNoMoveEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllDisableAi.ToString(), () => { IsDisableAiEnabled = !IsDisableAiEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllNoPostureBuildup.ToString(),
            () => { IsNoPostureBuildupEnabled = !IsNoPostureBuildupEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.AllTargetingView.ToString(),
            () => { IsTargetingViewEnabled = !IsTargetingViewEnabled; });
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsNoButterflySummonsEnabled) _enemyService.ToggleButterflyNoSummons(true);
        if (IsNoDeathEnabled) _enemyService.ToggleNoDeath(true);
        if (IsNoDamageEnabled) _enemyService.ToggleNoDamage(true);
        if (IsNoHitEnabled) _enemyService.ToggleNoHit(true);
        if (IsNoAttackEnabled) _enemyService.ToggleNoAttack(true);
        if (IsNoMoveEnabled) _enemyService.ToggleNoMove(true);
        if (IsDisableAiEnabled) _enemyService.ToggleDisableAi(true);
        if (IsNoPostureBuildupEnabled) _enemyService.ToggleNoPostureBuildup(true);
        if (IsDragonCombo1Enabled) _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo1, true);
        if (IsDragonCombo2Enabled) _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo2, true);
        if (IsDragonCombo3Enabled) _enemyService.ToggleDragonActCombo(AiActs.Dragon.Combo3, true);
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void SkipDragonPhaseOne() => _enemyService.SkipDragonPhaseOne();
    
    private void TriggerFinalDragonAttack()
    {
        if (!_eventService.GetEvent(GameEvent.HasDragonPhase2TreesSpawned)) return;
        _eventService.SetEvent(GameEvent.TriggerFinalDragonAttack, true);
    }

    #endregion
}