using System.Windows.Threading;
using SekiroTool.Enums;
using SekiroTool.Interfaces;

namespace SekiroTool.ViewModels;

public class EnemyViewModel : BaseViewModel
{
    private readonly IEnemyTargetService _enemyTargetService;

    private readonly DispatcherTimer _targetTick;
    
    private bool _areOptionsEnabled;
    private bool _isTargetOptionsEnabled;
    
    private ulong _currentTargetAddr;
    
    private int _customHp;
    private bool _customHpHasBeenSet;
    private int _targetCurrentHealth;
    private int _targetMaxHealth;
    
    private int _customPosture;
    private bool _customPostureHasBeenSet;
    private int _targetCurrentPosture;
    private int _targetMaxPosture;
    
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

        _targetTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(64)
        };
        _targetTick.Tick += TargetTick;
    }

   

    #region Commands
    
   

    #endregion

    #region Public Properties

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
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
        // IsValidTarget = true;
        
        
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

    #endregion
}