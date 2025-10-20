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

    private bool _isNoButterflySummonsEnabled;

    public bool IsNoButterflySummonsEnabled
    {
        get => _isNoButterflySummonsEnabled;
        set
        {
            SetProperty(ref _isNoButterflySummonsEnabled, value); 
            _enemyService.ToggleButterflyNoSnap(_isNoButterflySummonsEnabled);
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
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void SkipDragonPhaseOne() => _enemyService.SkipDragonPhaseOne();

    #endregion
}