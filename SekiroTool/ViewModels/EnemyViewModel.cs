using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyService _enemyService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IDebugDrawService _debugDrawService;

    public EnemyViewModel(IEnemyService enemyService, HotkeyManager hotkeyManager, IGameStateService gameStateService,
        IDebugDrawService debugDrawService)
    {
        _enemyService = enemyService;
        _hotkeyManager = hotkeyManager;
        _debugDrawService = debugDrawService;

        RegisterHotkeys();

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

        SkipDragonPhaseOneCommand = new DelegateCommand(SkipDragonPhaseOne);
    }


    #region Commands

    public ICommand SkipDragonPhaseOneCommand { get; set; }

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
            _enemyService.ToggleTargetingView(_isTargetingViewEnabled);
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
        if (IsTargetingViewEnabled) _enemyService.ToggleTargetingView(true);
        
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void SkipDragonPhaseOne() => _enemyService.SkipDragonPhaseOne();

    #endregion
}