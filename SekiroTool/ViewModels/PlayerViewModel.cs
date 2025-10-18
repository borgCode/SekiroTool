using System.Configuration;
using System.Windows.Threading;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class PlayerViewModel : BaseViewModel
{
    private readonly IPlayerService _playerService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IGameStateService _gameStateService;

    private readonly DispatcherTimer _playerTick;
    
    public PlayerViewModel(IPlayerService playerService, HotkeyManager hotkeyManager, IGameStateService gameStateService)
    {
        _playerService = playerService;
        _hotkeyManager = hotkeyManager;
        _gameStateService = gameStateService;

        RegisterHotkeys();
        
        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

        _playerTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(64)
        };
        _playerTick.Tick += PlayerTick;

    }

    


    #region Commands

    // Check TargetViewModel for examples of commands when you need to implement that

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
            if (SetProperty(ref _isNoDeathEnabled, value))
            {
                _playerService.TogglePlayerNoDeath(_isNoDeathEnabled);
            }
        }
    }
    
    // TODO PlayerNoDamage
    
    private bool _isOneShotEnabled;

    public bool IsOneShotEnabled
    {
        get => _isOneShotEnabled;
        set
        {
            if (SetProperty(ref _isOneShotEnabled, value))
            {
                _playerService.TogglePlayerOneShotHealth(_isOneShotEnabled);
            }    
        }
    }
    
    private bool _isOneShotPostureEnabled;

    public bool IsOneShotPostureEnabled
    {
        get => _isOneShotPostureEnabled;
        set
        {
            if (SetProperty(ref _isOneShotPostureEnabled, value))
            {
                _playerService.TogglePlayerOneShotPosture(_isOneShotPostureEnabled);
            }
        }
    }
    
    private bool _isNoGoodsConsumeEnabled;

    public bool IsNoGoodsConsumeEnabled
    {
        get => _isNoGoodsConsumeEnabled;
        set
        {
            if (SetProperty(ref _isNoGoodsConsumeEnabled, value))
            {
                _playerService.TogglePlayerNoGoodsConsume(_isNoGoodsConsumeEnabled);
            }
        }
    }
    
    private bool _isNoEmblemConsumeEnabled;

    public bool IsNoEmblemConsumeEnabled
    {
        get => _isNoEmblemConsumeEnabled;
        set
        {
            if (SetProperty(ref _isNoEmblemConsumeEnabled, value))
            {
                _playerService.TogglePlayerNoEmblemsConsume(_isNoEmblemConsumeEnabled);
            }
        }
    }
    
    private bool _isNoRevivalConsumeEnabled;

    public bool IsNoRevivalConsumeEnabled
    {
        get => _isNoRevivalConsumeEnabled;
        set
        {
            if (SetProperty(ref _isNoRevivalConsumeEnabled, value))
            {
                _playerService.TogglePlayerNoRevivalConsume(_isNoRevivalConsumeEnabled);    
            }
        }
    }
    
    private bool _isPlayerHideEnabled;

    public bool IsPlayerHideEnabled
    {
        get => _isPlayerHideEnabled;
        set
        {
            if (SetProperty(ref _isPlayerHideEnabled, value))
            {
                _playerService.TogglePlayerHide(_isPlayerHideEnabled);
            }
        }
    }
    
    private bool _isPlayerSilentEnabled;

    public bool IsPlayerSilentEnabled
    {
        get => _isPlayerSilentEnabled;
        set
        {
            if (SetProperty(ref _isPlayerSilentEnabled, value))
            {
                _playerService.TogglePlayerSilent(_isPlayerSilentEnabled);
            }
        }
    }
    
    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        // Check targetviewmodel when you are ready to implement hotkeys
    }
    
    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsNoDeathEnabled) _playerService.TogglePlayerNoDeath(true); 
        //TODO No Damage
        
        if (IsOneShotEnabled) _playerService.TogglePlayerOneShotHealth(true);
        
        if (IsOneShotPostureEnabled) _playerService.TogglePlayerOneShotPosture(true);

        if (IsNoGoodsConsumeEnabled) _playerService.TogglePlayerNoGoodsConsume(true);

        if (IsNoEmblemConsumeEnabled) _playerService.TogglePlayerNoGoodsConsume(true);
        
        if (_isNoRevivalConsumeEnabled) _playerService.TogglePlayerNoRevivalConsume(true);
        
        if (_isPlayerHideEnabled) _playerService.TogglePlayerHide(true);
        
        if (_isPlayerSilentEnabled) _playerService.TogglePlayerSilent(true);
    }
    
    
    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void PlayerTick(object? sender, EventArgs e)
    {
        // We'll have logic such as reading hp every tick etc, see how it works in targetviewmodel
    }

    #endregion
}