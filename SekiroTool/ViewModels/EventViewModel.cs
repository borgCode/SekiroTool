using System.Windows.Input;
using System.Windows.Media;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;

namespace SekiroTool.ViewModels;

public class EventViewModel : BaseViewModel
{
    private readonly IEventService _eventService;
    private readonly IDebugDrawService _debugDrawService;

    public EventViewModel(IEventService eventService, IStateService stateService,
        IDebugDrawService debugDrawService)
    {
        _eventService = eventService;
        _debugDrawService = debugDrawService;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        
        SetEventCommand = new DelegateCommand(SetEvent);
        GetEventCommand = new DelegateCommand(GetEvent);
        SetDemonBellCommand = new DelegateCommand(SetDemonBell);
        SetNoKurosCharmCommand = new DelegateCommand(SetNoKurosCharm);
        MoveIsshinCommand = new DelegateCommand(MoveIsshinToCastle);
    }
    

    #region Commands

    public ICommand SetEventCommand { get; set; }
    public ICommand GetEventCommand { get; set; }
    public ICommand SetDemonBellCommand { get; set; }
    public ICommand SetNoKurosCharmCommand { get; set; }
    public ICommand MoveIsshinCommand { get; set; }

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
    
    private bool _isDisableEventsEnabled;
    public bool IsDisableEventsEnabled
    {
        get => _isDisableEventsEnabled;
        set
        {
            if (!SetProperty(ref _isDisableEventsEnabled, value)) return;
            _eventService.ToggleDisableEvent(_isDisableEventsEnabled);
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
        if (IsDisableEventsEnabled) _eventService.ToggleDisableEvent(true);
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

    private void SetDemonBell(object parameter) =>
        _eventService.SetEvent(GameEvent.IsDemonBellActivated, Convert.ToBoolean(parameter));

    private void SetNoKurosCharm(object parameter) =>
        _eventService.SetEvent(GameEvent.IsNoKurosCharm, Convert.ToBoolean(parameter));

    private void MoveIsshinToCastle() => _eventService.SetEvent(GameEvent.HasIsshinMovedToCastle, true);

    #endregion
}