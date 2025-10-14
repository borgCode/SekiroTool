using System.Windows.Input;
using System.Windows.Threading;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyTargetService _enemyTargetService;

    private readonly DispatcherTimer _targetTick;

    private bool _areOptionsEnabled;
    private bool _isTargetOptionsEnabled;
    private bool _isValidTarget;

    private ulong _currentTargetAddr;

    private int _customHp;
    private bool _customHpHasBeenSet;
    private int _targetCurrentHealth;
    private int _targetMaxHealth;
    private bool _isFreezeHealthEnabled;

    private int _customPosture;
    private bool _customPostureHasBeenSet;
    private int _targetCurrentPosture;
    private int _targetMaxPosture;
    private bool _isFreezePostureEnabled;
    
    private float _targetCurrentPoise;
    private float _targetMaxPoise;
    private float _targetPoiseTimer;
    private bool _showPoise;

    private int _targetCurrentPoison;
    private int _targetMaxPoison;
    private bool _showPoison;
    // private bool _isPoisonImmune;
    
    private int _targetCurrentBurn;
    private int _targetMaxBurn;
    private bool _showBurn;
    // private bool _isBleedImmune;
    
    private int _targetCurrentShock;
    private int _targetMaxShock;
    private bool _showShock;
    // private bool _isToxicImmune;
    
    private bool _showAllResistances;

    private float _targetSpeed;

    private int _forceAct;
    private int _lastAct;
    private int _forceKengekiAct;
    private int _lastKengekiAct;
    private bool _isRepeatActEnabled;
    private bool _isRepeatKengekiActEnabled;


    public EnemyViewModel(IGameStateService gameStateService, IEnemyTargetService enemyTargetService)
    {
        _enemyTargetService = enemyTargetService;

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);

        SetHpCommand = new DelegateCommand(SetHp);
        SetHpPercentageCommand = new DelegateCommand(SetHpPercentage);

        SetPostureCommand = new DelegateCommand(SetPosture);
        SetPosturePercentageCommand = new DelegateCommand(SetPosturePercentage);

        _targetTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(64)
        };
        _targetTick.Tick += TargetTick;
    }


    #region Commands

    public ICommand SetHpCommand { get; set; }
    public ICommand SetHpPercentageCommand { get; set; }

    public ICommand SetPostureCommand { get; set; }
    public ICommand SetPosturePercentageCommand { get; set; }

    #endregion

    #region Public Properties

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    public bool IsValidTarget
    {
        get => _isValidTarget;
        set => SetProperty(ref _isValidTarget, value);
    }

    public bool IsTargetOptionsEnabled
    {
        get => _isTargetOptionsEnabled;
        set
        {
            if (!SetProperty(ref _isTargetOptionsEnabled, value)) return;
            if (value)
            {
                _enemyTargetService.ToggleTargetHook(true);
                _targetTick.Start();
                ShowAllResistances = true;
            }
            else
            {
                _targetTick.Stop();
                // IsRepeatActEnabled = false;
                // IsCinderPhasedLocked = false;
                ShowAllResistances = false;
                // IsResistancesWindowOpen = false;
                // IsFreezeHealthEnabled = false;
                _enemyTargetService.ToggleTargetHook(false);
                ShowPoise = false;
                ShowPoison = false;
                ShowBurn = false;
                ShowShock = false;
            }
        }
    }

    public int CustomHp
    {
        get => _customHp;
        set
        {
            if (SetProperty(ref _customHp, value))
            {
                _customHpHasBeenSet = true;
            }
        }
    }

    public int TargetCurrentHealth
    {
        get => _targetCurrentHealth;
        set => SetProperty(ref _targetCurrentHealth, value);
    }

    public int TargetMaxHealth
    {
        get => _targetMaxHealth;
        set => SetProperty(ref _targetMaxHealth, value);
    }

    public bool IsFreezeHealthEnabled
    {
        get => _isFreezeHealthEnabled;
        set
        {
            SetProperty(ref _isFreezeHealthEnabled, value);
            _enemyTargetService.ToggleNoDamage(_isFreezeHealthEnabled);
        }
    }

    public int TargetCurrentPosture
    {
        get => _targetCurrentPosture;
        set => SetProperty(ref _targetCurrentPosture, value);
    }

    public int TargetMaxPosture
    {
        get => _targetMaxPosture;
        set => SetProperty(ref _targetMaxPosture, value);
    }

    public bool IsFreezePostureEnabled
    {
        get => _isFreezePostureEnabled;
        set
        {
            SetProperty(ref _isFreezePostureEnabled, value);
            _enemyTargetService.ToggleFreezePosture(_isFreezePostureEnabled);
        }
    }
    
    public float TargetCurrentPoise
    {
        get => _targetCurrentPoise;
        set => SetProperty(ref _targetCurrentPoise, value);
    }

    public float TargetMaxPoise
    {
        get => _targetMaxPoise;
        set => SetProperty(ref _targetMaxPoise, value);
    }

    public float TargetPoiseTimer
    {
        get => _targetPoiseTimer;
        set => SetProperty(ref _targetPoiseTimer, value);
    }

    public bool ShowPoise
    {
        get => _showPoise;
        set
        {
            SetProperty(ref _showPoise, value);
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }
    }

    
    public int TargetCurrentPoison
    {
        get => _targetCurrentPoison;
        set => SetProperty(ref _targetCurrentPoison, value);
    }

    public int TargetMaxPoison
    {
        get => _targetMaxPoison;
        set => SetProperty(ref _targetMaxPoison, value);
    }

    public bool ShowPoison
    {
        get => _showPoison;
        set
        {
            SetProperty(ref _showPoison, value);
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }
    }
    
    public int TargetCurrentBurn
    {
        get => _targetCurrentBurn;
        set => SetProperty(ref _targetCurrentBurn, value);
    }

    public int TargetMaxBurn
    {
        get => _targetMaxBurn;
        set => SetProperty(ref _targetMaxBurn, value);
    }

    public bool ShowBurn
    {
        get => _showBurn;
        set
        {
            SetProperty(ref _showBurn, value);
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }
    }
    
    public int TargetCurrentShock
    {
        get => _targetCurrentShock;
        set => SetProperty(ref _targetCurrentShock, value);
    }

    public int TargetMaxShock
    {
        get => _targetMaxShock;
        set => SetProperty(ref _targetMaxShock, value);
    }

    public bool ShowShock
    {
        get => _showShock;
        set
        {
            SetProperty(ref _showShock, value);
            // if (!IsResistancesWindowOpen || _resistancesWindowWindow == null) return;
            // _resistancesWindowWindow.DataContext = null;
            // _resistancesWindowWindow.DataContext = this;
        }
    }
    
    public bool ShowAllResistances
    {
        get => _showAllResistances;
        set
        {
            if (SetProperty(ref _showAllResistances, value))
            {
                UpdateResistancesDisplay();
            }
        }
    }
    
    public float TargetSpeed
    {
        get => _targetSpeed;
        set
        {
            if (SetProperty(ref _targetSpeed, value))
            {
                _enemyTargetService.SetSpeed(value);
            }
        }
    }
    
    public void SetSpeed(double value) => TargetSpeed = (float)value;

    public int LastAct
    {
        get => _lastAct;
        set => SetProperty(ref _lastAct, value);
    }

    public int ForceAct
    {
        get => _forceAct;
        set
        {
            if (!SetProperty(ref _forceAct, value)) return;
            _enemyTargetService.ForceAct(_forceAct);
            if (_forceAct == 0) IsRepeatActEnabled = false;
        }
    }

    public int LastKengekiAct
    {
        get => _lastKengekiAct;
        set => SetProperty(ref _lastKengekiAct, value);
    }

    public int ForceKengekiAct
    {
        get => _forceKengekiAct;
        set
        {
            if (!SetProperty(ref _forceKengekiAct, value)) return;
            _enemyTargetService.ForceKengekiAct(_forceKengekiAct);
            if (_forceKengekiAct == 0) IsRepeatKengekiActEnabled = false;
        }
    }

    public bool IsRepeatActEnabled
    {
        get => _isRepeatActEnabled;
        set
        {
            if (!SetProperty(ref _isRepeatActEnabled, value)) return;

            bool isRepeating = _enemyTargetService.IsTargetRepeating();

            switch (value)
            {
                case true when !isRepeating:
                    _enemyTargetService.ToggleTargetRepeatAct(true);
                    ForceAct = _enemyTargetService.GetLastAct();
                    break;
                case false when isRepeating:
                    _enemyTargetService.ToggleTargetRepeatAct(false);
                    ForceAct = 0;
                    break;
            }
        }
    }

    public bool IsRepeatKengekiActEnabled
    {
        get => _isRepeatKengekiActEnabled;
        set
        {
            if (!SetProperty(ref _isRepeatKengekiActEnabled, value)) return;

            bool isRepeating = _enemyTargetService.IsTargetRepeatingKengeki();

            switch (value)
            {
                case true when !isRepeating:
                    _enemyTargetService.ToggleTargetRepeatKengekiAct(true);
                    ForceKengekiAct = _enemyTargetService.GetLastKengekiAct();
                    break;
                case false when isRepeating:
                    _enemyTargetService.ToggleTargetRepeatKengekiAct(false);
                    ForceKengekiAct = 0;
                    break;
            }
        }
    }

    #endregion

    #region Private Methods

    private void TargetTick(object? sender, EventArgs e)
    {
        if (!IsTargetValid())
        {
            IsValidTarget = false;
            return;
        }

        IsValidTarget = true;


        ulong targetAddr = _enemyTargetService.GetTargetAddr();


        if (targetAddr != _currentTargetAddr)
        {
            _currentTargetAddr = targetAddr;
            
            TargetMaxHealth = _enemyTargetService.GetMaxHp();
            TargetMaxPosture = _enemyTargetService.GetMaxPosture();
            TargetMaxPoise = _enemyTargetService.GetMaxPoise();
            TargetMaxPoison = _enemyTargetService.GetMaxPoison();
            TargetMaxBurn = _enemyTargetService.GetMaxBurn();
            TargetMaxShock = _enemyTargetService.GetMaxShock();
        }

        TargetCurrentHealth = _enemyTargetService.GetCurrentHp();
        TargetCurrentPosture = _enemyTargetService.GetCurrentPosture();
        TargetCurrentPoise = _enemyTargetService.GetCurrentPoise();
        TargetPoiseTimer = _enemyTargetService.GetPoiseTimer();
        TargetCurrentPoison = _enemyTargetService.GetCurrentPoison();
        TargetCurrentBurn = _enemyTargetService.GetCurrentBurn();
        TargetCurrentShock = _enemyTargetService.GetCurrentShock();

        TargetSpeed = _enemyTargetService.GetSpeed();
        
        LastAct = _enemyTargetService.GetLastAct();
        LastKengekiAct = _enemyTargetService.GetLastKengekiAct();
    }

    private bool IsTargetValid()
    {
        ulong targetAddr = _enemyTargetService.GetTargetAddr();
        if (targetAddr == 0) return false;

        float health = _enemyTargetService.GetCurrentHp();
        float maxHealth = _enemyTargetService.GetMaxHp();
        if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000) return false;

        if (health > maxHealth * 1.5) return false;

        var position = _enemyTargetService.GetPosition();

        if (float.IsNaN(position[0]) || float.IsNaN(position[1]) || float.IsNaN(position[2])) return false;

        if (Math.Abs(position[0]) > 10000 || Math.Abs(position[1]) > 10000 || Math.Abs(position[2]) > 10000)
            return false;

        return true;
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }

    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void SetHp(object parameter) =>
        _enemyTargetService.SetHp(Convert.ToInt32(parameter));

    private void SetHpPercentage(object parameter)
    {
        int healthPercentage = Convert.ToInt32(parameter);
        int newHealth = TargetMaxHealth * healthPercentage / 100;
        _enemyTargetService.SetHp(newHealth);
    }

    private void SetPosture(object parameter) =>
        _enemyTargetService.SetPosture(Convert.ToInt32(parameter));

    private void SetPosturePercentage(object parameter)
    {
        int posturePercentage = Convert.ToInt32(parameter);
        int newPosture = TargetMaxPosture * posturePercentage / 100;
        _enemyTargetService.SetPosture(newPosture);
    }

    private void UpdateResistancesDisplay()
    {
        if (!IsTargetOptionsEnabled) return;
        if (_showAllResistances)
        {
            ShowPoise = true;
            ShowPoison = true;
            ShowBurn = true;
            ShowShock = true;
        }
        else
        {
            ShowPoise = false;
            ShowPoison = false;
            ShowBurn = false;
            ShowShock = false;
        }
    }

    #endregion

    
}