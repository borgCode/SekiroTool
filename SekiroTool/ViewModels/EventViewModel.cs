using System.Windows;
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

    public static readonly Brush ActiveButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#963839"));
    public static readonly Brush DefaultButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#252525"));
    
    public EventViewModel(IEventService eventService, IStateService stateService,
        IDebugDrawService debugDrawService)
    {
        _eventService = eventService;
        _debugDrawService = debugDrawService;

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.EventTabActivated, OnEventTabActivated);

        SetEventCommand = new DelegateCommand(SetEvent);
        GetEventCommand = new DelegateCommand(GetEvent);
        SetDemonBellCommand = new DelegateCommand(SetDemonBell);
        SetNoKurosCharmCommand = new DelegateCommand(SetNoKurosCharm);
        MoveIsshinCommand = new DelegateCommand(MoveIsshinToCastle);
        SetMorningCommand = new DelegateCommand(SetMorning);
        SetNoonCommand = new DelegateCommand(SetNoon);
        SetEveningCommand = new DelegateCommand(SetEvening);
        SetNightCommand = new DelegateCommand(SetNight);
        SetTutorialCompleteCommand = new DelegateCommand(SetTutorialComplete);
        SetHirataTwoCommand = new DelegateCommand(SetHirataTwo);
        SetInvasionCommand = new DelegateCommand(SetInvasion);
        SetAshinaNightCommand = new DelegateCommand(SetAshinaNight);
        SetHeadlessApeCommand = new DelegateCommand(SetHeadlessApe);
    }
    
    #region Commands

    public ICommand SetEventCommand { get; set; }
    public ICommand GetEventCommand { get; set; }
    public ICommand SetDemonBellCommand { get; set; }
    public ICommand SetNoKurosCharmCommand { get; set; }
    public ICommand MoveIsshinCommand { get; set; }
    public ICommand SetMorningCommand { get; set; }
    public ICommand SetNoonCommand { get; set; }
    public ICommand SetEveningCommand { get; set; }
    public ICommand SetNightCommand { get; set; }
    public ICommand SetTutorialCompleteCommand { get; set; }
    public ICommand SetHirataTwoCommand { get; set; }
    public ICommand SetInvasionCommand { get; set; }
    public ICommand SetAshinaNightCommand { get; set; }
    public ICommand SetHeadlessApeCommand { get; set; }

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
    
    private Brush _morningBackground;
    public Brush MorningBackground
    {
        get => _morningBackground;
        set => SetProperty(ref _morningBackground, value);
    }

    private Brush _noonBackground;
    public Brush NoonForeground
    {
        get => _noonBackground;
        set => SetProperty(ref _noonBackground, value);
    }

    private Brush _eveningBackground;
    public Brush EveningBackground
    {
        get => _eveningBackground;
        set => SetProperty(ref _eveningBackground, value);
    }

    private Brush _nightBackground;
    public Brush NightBackground
    {
        get => _nightBackground;
        set => SetProperty(ref _nightBackground, value);
    }
    
    private Brush _demonBellOnBackground;
    public Brush DemonBellOnBackground
    {
        get => _demonBellOnBackground;
        set => SetProperty(ref _demonBellOnBackground, value);
    }

    private Brush _demonBellOffBackground;
    public Brush DemonBellOffBackground
    {
        get => _demonBellOffBackground;
        set => SetProperty(ref _demonBellOffBackground, value);
    }
    
    private Brush _noKurosCharmOnBackground;
    public Brush NoKurosCharmOnForeground
    {
        get => _noKurosCharmOnBackground;
        set => SetProperty(ref _noKurosCharmOnBackground, value);
    }

    private Brush _noKurosCharmOffBackground;
    public Brush NoKurosCharmOffBackground
    {
        get => _noKurosCharmOffBackground;
        set => SetProperty(ref _noKurosCharmOffBackground, value);
    }

    private Brush _tutorialCompleteOnBackground;
    public Brush TutorialCompleteOnBackground
    {
        get => _tutorialCompleteOnBackground;
        set => SetProperty(ref _tutorialCompleteOnBackground, value);
    }

    private Brush _tutorialCompleteOffBackground;
    public Brush TutorialCompleteOffBackground
    {
        get => _tutorialCompleteOffBackground;
        set => SetProperty(ref _tutorialCompleteOffBackground, value);
    }
    
    private Brush _hirataTwoOnBackground;
    public Brush HirataTwoOnBackground
    {
        get => _hirataTwoOnBackground;
        set => SetProperty(ref _hirataTwoOnBackground, value);
    }

    private Brush _hirataTwoOffBackground;
    public Brush HirataTwoOffBackground
    {
        get => _hirataTwoOffBackground;
        set => SetProperty(ref _hirataTwoOffBackground, value);
    }
    
    private Brush _ashinaInvasionOnBackground;
    public Brush AshinaInvasionOnBackground
    {
        get => _ashinaInvasionOnBackground;
        set => SetProperty(ref _ashinaInvasionOnBackground, value);
    }

    private Brush _ashinaInvasionOffBackground;
    public Brush AshinaInvasionOffBackground
    {
        get => _ashinaInvasionOffBackground;
        set => SetProperty(ref _ashinaInvasionOffBackground, value);
    }
    
    private Brush _ashinaNightOnBackground;
    public Brush AshinaNightOnBackground
    {
        get => _ashinaNightOnBackground;
        set => SetProperty(ref _ashinaNightOnBackground, value);
    }

    private Brush _ashinaNightOffBackground;
    public Brush AshinaNightOffBackground
    {
        get => _ashinaNightOffBackground;
        set => SetProperty(ref _ashinaNightOffBackground, value);
    }
    
    private Brush _headlessApeOnBackground;
    public Brush HeadlessApeOnBackground
    {
        get => _headlessApeOnBackground;
        set => SetProperty(ref _headlessApeOnBackground, value);
    }

    private Brush _headlessApeOffBackground;
    public Brush HeadlessApeOffBackground
    {
        get => _headlessApeOffBackground;
        set => SetProperty(ref _headlessApeOffBackground, value);
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
        UpdateAllEventStatus();
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }
    
    private void OnEventTabActivated()
    {
        if (!AreOptionsEnabled) return;
        UpdateAllEventStatus();
    }
    
    private void UpdateAllEventStatus()
    {
        UpdateScalingStatus();
        UpdateDemonBellStatus();
        UpdateNoKurosCharmStatus();
        UpdateGameEvents();
    }

    private void UpdateScalingStatus()
    {
        bool isNoon = _eventService.GetEvent(GameEvent.NoonScaling);
        bool isEvening = _eventService.GetEvent(GameEvent.EveningScaling);
        bool isNight = _eventService.GetEvent(GameEvent.NightScaling);
        
        if (!isNoon && !isEvening && !isNight)
        {
            MorningBackground = ActiveButtonColor;
            NoonForeground = DefaultButtonColor;
            EveningBackground = DefaultButtonColor;
            NightBackground = DefaultButtonColor;
        }
        else if (isNoon && !isEvening && !isNight)
        {
            MorningBackground = DefaultButtonColor;
            NoonForeground = ActiveButtonColor;
            EveningBackground = DefaultButtonColor;
            NightBackground = DefaultButtonColor;
        }
        else if (isNoon && isEvening && !isNight)
        {
            MorningBackground = DefaultButtonColor;
            NoonForeground = DefaultButtonColor;
            EveningBackground = ActiveButtonColor;
            NightBackground = DefaultButtonColor;
        }
        else if (isNoon && isEvening && isNight)
        {
            MorningBackground = DefaultButtonColor;
            NoonForeground = DefaultButtonColor;
            EveningBackground = DefaultButtonColor;
            NightBackground = ActiveButtonColor;
        }
    }

    private void UpdateDemonBellStatus()
    {
        bool isDemonBellOn = _eventService.GetEvent(GameEvent.IsDemonBellActivated);
        DemonBellOnBackground = isDemonBellOn ? ActiveButtonColor : DefaultButtonColor;
        DemonBellOffBackground = !isDemonBellOn ? ActiveButtonColor : DefaultButtonColor;
    }
    
    private void UpdateNoKurosCharmStatus()
    {
                
        bool isNoKurosCharm = _eventService.GetEvent(GameEvent.IsNoKurosCharm);
        NoKurosCharmOnForeground = isNoKurosCharm ? ActiveButtonColor : DefaultButtonColor;
        NoKurosCharmOffBackground = !isNoKurosCharm ? ActiveButtonColor : DefaultButtonColor;
    }
    
    private void UpdateGameEvents()
    {
        bool isTutorialComplete = !_eventService.GetEvent(GameEvent.IsTutorial);
        TutorialCompleteOnBackground = isTutorialComplete ? ActiveButtonColor : DefaultButtonColor;
        TutorialCompleteOffBackground = !isTutorialComplete ? ActiveButtonColor : DefaultButtonColor;
        
        bool isHirataTwo = _eventService.GetEvent(GameEvent.HirataFire);
        HirataTwoOnBackground = isHirataTwo ? ActiveButtonColor : DefaultButtonColor;
        HirataTwoOffBackground = !isHirataTwo ? ActiveButtonColor : DefaultButtonColor;
        
        bool isInvasion = _eventService.GetEvent(GameEvent.AshinaCastleInvasion);
        AshinaInvasionOnBackground = isInvasion ? ActiveButtonColor : DefaultButtonColor;
        AshinaInvasionOffBackground = !isInvasion ? ActiveButtonColor : DefaultButtonColor;
        
        bool isAshinaNight = _eventService.GetEvent(GameEvent.AshinaCastleFire);
        AshinaNightOnBackground = isAshinaNight ? ActiveButtonColor : DefaultButtonColor;
        AshinaNightOffBackground = !isAshinaNight ? ActiveButtonColor : DefaultButtonColor;
        
        bool isHeadlessApe = _eventService.GetEvent(GameEvent.HeadlessApe);
        HeadlessApeOnBackground = isHeadlessApe ? ActiveButtonColor : DefaultButtonColor;
        HeadlessApeOffBackground = !isHeadlessApe ? ActiveButtonColor : DefaultButtonColor;
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

    private void SetDemonBell(object parameter)
    { 
        _eventService.SetEvent(GameEvent.IsDemonBellActivated, Convert.ToBoolean(parameter));
        UpdateDemonBellStatus();
    }


    private void SetNoKurosCharm(object parameter)
    {
        _eventService.SetEvent(GameEvent.IsNoKurosCharm, Convert.ToBoolean(parameter));
        UpdateNoKurosCharmStatus();
    }
    
    private void MoveIsshinToCastle() => _eventService.SetEvent(GameEvent.HasIsshinMovedToCastle, true);

    private void SetMorning()
    {
        _eventService.SetEvent(GameEvent.NoonScaling, false);
        _eventService.SetEvent(GameEvent.EveningScaling, false);
        _eventService.SetEvent(GameEvent.NightScaling, false);
        UpdateScalingStatus();
    }

    private void SetNoon()
    {
        _eventService.SetEvent(GameEvent.NoonScaling, true);
        _eventService.SetEvent(GameEvent.EveningScaling, false);
        _eventService.SetEvent(GameEvent.NightScaling, false);
        UpdateScalingStatus();

    }

    private void SetEvening()
    {
        _eventService.SetEvent(GameEvent.NoonScaling, true);
        _eventService.SetEvent(GameEvent.EveningScaling, true);
        _eventService.SetEvent(GameEvent.NightScaling, false);
        UpdateScalingStatus();

    }

    private void SetNight()
    {
        _eventService.SetEvent(GameEvent.NoonScaling, true);
        _eventService.SetEvent(GameEvent.EveningScaling, true);
        _eventService.SetEvent(GameEvent.NightScaling, true);
        UpdateScalingStatus();

    }

    private void SetTutorialComplete(object parameter) => 
        _eventService.SetEvent(GameEvent.IsTutorial, Convert.ToBoolean(parameter));


    private void SetHirataTwo(object parameter)
    {
        bool isOn = Convert.ToBoolean(parameter);
        if (isOn) _eventService.SetEvent(GameEvent.IsTutorial, false);
        _eventService.SetEvent(GameEvent.HirataFire, isOn);
    }

    private void SetInvasion(object parameter)
    {
        bool isOn = Convert.ToBoolean(parameter);
        if (isOn) _eventService.SetEvent(GameEvent.IsTutorial, false);
        _eventService.SetEvent(GameEvent.AshinaCastleInvasion, isOn);
        
    }

    private void SetAshinaNight(object parameter)
    {
        bool isOn = Convert.ToBoolean(parameter);
        if (isOn) _eventService.SetEvent(GameEvent.IsTutorial, false);
        _eventService.SetEvent(GameEvent.AshinaCastleFire, isOn);
    }

    private void SetHeadlessApe(object parameter)=> 
        _eventService.SetEvent(GameEvent.HeadlessApe, Convert.ToBoolean(parameter));

    #endregion
}