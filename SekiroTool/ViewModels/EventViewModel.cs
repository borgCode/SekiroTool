using System.Windows.Input;
using System.Windows.Media;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.ViewModels;

public class EventViewModel : BaseViewModel
{
    private readonly IEventService _eventService;
    private readonly IDebugDrawService _debugDrawService;

    public EventViewModel(IEventService eventService, IGameStateService gameStateService,
        IDebugDrawService debugDrawService)
    {
        _eventService = eventService;
        _debugDrawService = debugDrawService;

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);
        
        SetEventCommand = new DelegateCommand(SetEvent);
        GetEventCommand = new DelegateCommand(GetEvent);
    }
    
    #region Commands

    public ICommand SetEventCommand { get; set; }
    public ICommand GetEventCommand { get; set; }

    #endregion
    
    #region Properties

    private bool _areOptionsEnabled;
    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }
    
    private string _setFlagId;
    public string SetFlagId
    {
        get => _setFlagId;
        set => SetProperty(ref _setFlagId, value);
    }
    
    private string _getFlagId;
    public string GetFlagId
    {
        get => _getFlagId;
        set => SetProperty(ref _getFlagId, value);
    }
    
    private int _flagStateIndex;
    public int FlagStateIndex
    {
        get => _flagStateIndex;
        set => SetProperty(ref _flagStateIndex, value);
    }
    
    private string _eventStatusText;
    public string EventStatusText
    {
        get => _eventStatusText;
        set => SetProperty(ref _eventStatusText, value);
    }
    
    private Brush _eventStatusColor;
    public Brush EventStatusColor
    {
        get => _eventStatusColor;
        set => SetProperty(ref _eventStatusColor, value);
    }

    private bool _isDrawEventsEnabled;
    public bool IsDrawEventsEnabled
    {
        get => _isDrawEventsEnabled;
        set
        {
            if (!SetProperty(ref _isDrawEventsEnabled, value)) return;
            if (_isDrawEventsEnabled) _debugDrawService.RequestDebugDraw();
            else _debugDrawService.ReleaseDebugDraw();
            _eventService.ToggleDrawEvents(_isDrawEventsEnabled);
        }
    }
    
    #endregion
    
    #region Private Methods

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsDrawEventsEnabled)
        {
            _debugDrawService.RequestDebugDraw();
            _eventService.ToggleDrawEvents(true);
        }
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void SetEvent()
    {
        if (string.IsNullOrWhiteSpace(SetFlagId))
            return;
            
        string trimmedFlagId = SetFlagId.Trim();
        
        if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
            return;
        _eventService.SetEvent(flagIdValue, FlagStateIndex == 0);
    }

    private void GetEvent()
    {
        if (string.IsNullOrWhiteSpace(GetFlagId))
            return;
            
        string trimmedFlagId = GetFlagId.Trim();
            
        if (!long.TryParse(trimmedFlagId, out long flagIdValue) || flagIdValue <= 0)
            return;

        if (_eventService.GetEvent(flagIdValue))
        {
            EventStatusText = "True";
            EventStatusColor = Brushes.Chartreuse;
        }
        else
        {
            EventStatusText = "False";
            EventStatusColor = Brushes.Red;
        }
    }

    #endregion
}