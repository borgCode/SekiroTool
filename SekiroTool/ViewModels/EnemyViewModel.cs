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
    
    private float _targetSpeed;

    private int _forceAct;
    private int _lastAct;
    private int _forceKengekiAct;
    private int _lastKengekiAct;
    private bool _isRepeatActEnabled;
    private bool _isRepeatKegekiActEnabled;


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
                // ShowAllResistances = true;
            }
            else
            {
                _targetTick.Stop();
                // IsRepeatActEnabled = false;
                // IsCinderPhasedLocked = false;
                // ShowAllResistances = false;
                // IsResistancesWindowOpen = false;
                // IsFreezeHealthEnabled = false;
                _enemyTargetService.ToggleTargetHook(false);
                // ShowPoise = false;
                // ShowBleed = false;
                // ShowPoison = false;
                // ShowFrost = false;
                // ShowToxic = false;
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
            if (_forceKengekiAct == 0) IsRepeatKegekiActEnabled = false;
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
    
    public bool IsRepeatKegekiActEnabled
    {
        get => _isRepeatKegekiActEnabled;
        set
        {
            if (!SetProperty(ref _isRepeatKegekiActEnabled, value)) return;

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
        // if (!IsTargetValid())
        // {
        //     IsValidTarget = false;
        //     return;
        // }
        //
        IsValidTarget = true;
        
        
        ulong targetAddr = _enemyTargetService.GetTargetAddr();
        
        
        if (targetAddr != _currentTargetAddr)
        {
            _currentTargetAddr = targetAddr;
        }
        
        TargetCurrentHealth = _enemyTargetService.GetCurrentHp();
        TargetMaxHealth = _enemyTargetService.GetMaxHp();

        TargetCurrentPosture = _enemyTargetService.GetCurrentPosture();
        TargetMaxPosture = _enemyTargetService.GetMaxPosture();

        LastAct = _enemyTargetService.GetLastAct();
        LastKengekiAct = _enemyTargetService.GetLastKengekiAct();
        
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

    #endregion
}