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