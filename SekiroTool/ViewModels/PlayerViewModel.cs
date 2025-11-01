using System.Configuration;
using System.Windows.Input;
using System.Windows.Threading;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using Xceed.Wpf.Toolkit;

namespace SekiroTool.ViewModels;

public class PlayerViewModel : BaseViewModel
{
    private readonly IPlayerService _playerService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IGameStateService _gameStateService;

    private readonly DispatcherTimer _playerTick;

    private bool _pauseUpdates;
    
    public PlayerViewModel(IPlayerService playerService, HotkeyManager hotkeyManager, IGameStateService gameStateService)
    {
        _playerService = playerService;
        _hotkeyManager = hotkeyManager;
        _gameStateService = gameStateService;

        RegisterHotkeys();
        
        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);
        
        SavePositionCommand = new DelegateCommand(SavePosition);
        RestorePositionCommand = new DelegateCommand(RestorePosition);

        _playerTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(64)
        };
        _playerTick.Tick += PlayerTick;
        

    }
    
    #region Commands
    
    public ICommand SavePositionCommand { get; set; }
    public ICommand RestorePositionCommand { get; set; }

    // Check TargetViewModel for examples of commands when you need to implement that

    #endregion


    #region Properties

    private bool _areOptionsEnabled;
    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }
    private float _posX;
    public float PosX
    {
        get => _posX;
        set => SetProperty(ref _posX, value);
    }
    
    private float _posY;
    public float PosY
    {
        get => _posY;
        set => SetProperty(ref _posY, value);
    }
    
    private float _posZ;
    public float PosZ
    {
        get => _posZ;
        set => SetProperty(ref _posZ, value);
    }
    
    private bool _isPos1Saved;
    public bool IsPos1Saved
    {
        get => _isPos1Saved;
        set => SetProperty(ref _isPos1Saved, value);
    }

    private bool _isPos2Saved;
    public bool IsPos2Saved
    {
        get => _isPos2Saved;
        set => SetProperty(ref _isPos2Saved, value);
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

    private bool _isInfinitePoiseEnabled;
    public bool IsInfinitePoiseEnabled
    {
        get => _isInfinitePoiseEnabled;
        set
        {
            if (SetProperty(ref _isInfinitePoiseEnabled, value))
            {
                _playerService.TogglePlayerInfinitePoise(_isInfinitePoiseEnabled);
            }
        }
    }
    
    private bool _isInfiniteConfettiEnabled;

    public bool IsInfiniteConfettiEnabled
    {
        get => _isInfiniteConfettiEnabled;
        set
        {
            if (SetProperty(ref _isInfiniteConfettiEnabled, value))
            {
                _playerService.ToggleInfiniteConfetti(_isInfiniteConfettiEnabled);
            }
        }
    }
    
    private int _newGame;
    public int NewGame
    {
        get => _newGame;
        set
        {
            if (SetProperty(ref _newGame, value))
            {
                _playerService.SetNewGame(value);
            }
        }
    }
    
    #endregion

    #region Public Methods

    public void PauseUpdates() => _pauseUpdates = true;
    public void ResumeUpdates() => _pauseUpdates = false;
    public void SetNewGame(int newGameCycle) => _playerService.SetNewGame(newGameCycle);

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.SavePos1.ToString(), () => SavePosition(0));
        _hotkeyManager.RegisterAction(HotkeyActions.SavePos2.ToString(), () => SavePosition(1));
        _hotkeyManager.RegisterAction(HotkeyActions.RestorePos1.ToString(), () => RestorePosition(0));
        _hotkeyManager.RegisterAction(HotkeyActions.RestorePos2.ToString(), () => RestorePosition(1));
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
        
        if (_isInfinitePoiseEnabled) _playerService.TogglePlayerInfinitePoise(true);
        _playerTick.Start();
        
        if (_isInfiniteConfettiEnabled) _playerService.ToggleInfiniteConfetti(true);
        _playerTick.Start();
        
        
        
    }
    
    
    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        _playerTick.Stop();
    }
    
    private void PlayerTick(object? sender, EventArgs e)
    {
        if (_pauseUpdates) return;
        var coords = _playerService.GetCoords();
        PosX = coords.x;
        PosY = coords.y;
        PosZ = coords.z;
        NewGame = _playerService.GetNewGame();

        // We'll have logic such as reading hp every tick etc, see how it works in targetviewmodel
    }
    
    
    private void SavePosition(object parameter)
    {
        int index = Convert.ToInt32(parameter);
        if (index == 0) IsPos1Saved = true;
        else IsPos2Saved = true;
        //TODO include state
        
        _playerService.SavePos(index);
    }

    private void RestorePosition(object parameter)
    {
        int index = Convert.ToInt32(parameter);
        _playerService.RestorePos(index);
        //TODO include state
    }

    #endregion
    
}