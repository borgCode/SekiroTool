using System.Configuration;
using System.Windows.Input;
using System.Windows.Threading;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;
using Xceed.Wpf.Toolkit;

namespace SekiroTool.ViewModels;

public class PlayerViewModel : BaseViewModel
{
    private readonly IPlayerService _playerService;
    private readonly HotkeyManager _hotkeyManager;

    private readonly DispatcherTimer _playerTick;

    private Dictionary<uint, uint> _idolsByAreaDict;

    private bool _pauseUpdates;

    private const float DefaultDamageMultiplier = 1.0f;
    
    private const float DefaultSpeed = 1f;
    private float _playerDesiredSpeed = -1f;
    private const float Epsilon = 0.0001f;

    public PlayerViewModel(IPlayerService playerService, HotkeyManager hotkeyManager,
        IStateService stateService)
    {
        _playerService = playerService;
        _hotkeyManager = hotkeyManager;

        RegisterHotkeys();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.GameStart, OnGameStart);

        SavePositionCommand = new DelegateCommand(SavePosition);
        RestorePositionCommand = new DelegateCommand(RestorePosition);

        SetMaxHpCommand = new DelegateCommand(SetMaxHp);
        SetMaxPostureCommand = new DelegateCommand(SetMaxPosture);
        RemoveConfettiCommand = new DelegateCommand(SetRemoveConfetti);
        RemoveGachiinCommand = new DelegateCommand(SetRemoveGachiin);
        SetRestCommand = new DelegateCommand(Rest);
        SetApplyConfettiCommand = new DelegateCommand(SetApplyConfetti);
        SetApplyGachiinCommand = new DelegateCommand(SetApplyGachiin);
        SetAddSenCommand = new DelegateCommand(SetAddSen);
        SetAddExperienceCommand = new DelegateCommand(SetAddExperience);
        _playerTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(64)
        };
        _playerTick.Tick += PlayerTick;

        _idolsByAreaDict = DataLoader.GetIdolsByAreaDictionary();
    }

    
    #region Commands

    public ICommand SavePositionCommand { get; set; }
    public ICommand RestorePositionCommand { get; set; }

    public ICommand SetMaxHpCommand { get; set; }
    
    public ICommand SetMaxPostureCommand { get; set; }
    
    public ICommand RemoveConfettiCommand { get; set; }
    
    public ICommand RemoveGachiinCommand { get; set; }
    
    public ICommand SetRestCommand { get; set; }

    public ICommand SetApplyConfettiCommand {  get; set; }
    
    public ICommand SetApplyGachiinCommand {  get; set; }
    
    public ICommand SetAddSenCommand {  get; set; }
    public ICommand SetAddExperienceCommand {  get; set; }
    
    
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
                if (IsNoDeathEnabledWithoutKillbox && _isNoDeathEnabled)
                {
                   IsNoDeathEnabledWithoutKillbox = false; 
                }
                _playerService.TogglePlayerNoDeath(_isNoDeathEnabled);
            }
            
        }
    }

    private bool _isNoDamageEnabled;

    public bool IsNoDamageEnabled
    {
        get => _isNoDamageEnabled;
        set
        {
            if (SetProperty(ref _isNoDamageEnabled, value))
            {
                _playerService.TogglePlayerNoDamage(_isNoDamageEnabled);
            }
        }
    }
    
    private bool _isNoDeathEnabledWithoutKillbox;
    public bool IsNoDeathEnabledWithoutKillbox
    {
        get => _isNoDeathEnabledWithoutKillbox;
        set 
        { 
            if (SetProperty(ref _isNoDeathEnabledWithoutKillbox, value))
            {
                if (IsNoDeathEnabled && _isNoDeathEnabledWithoutKillbox)
                {
                    IsNoDeathEnabled = false; 
                }
                _playerService.TogglePlayerNoDeathWithoutKillbox(_isNoDeathEnabledWithoutKillbox); 
            }
        }
    }
    

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
    

    private bool _isConfettiFlagEnabled;

    public bool IsConfettiFlagEnabled
    {
        get => _isConfettiFlagEnabled;
        set
        {
            if (SetProperty(ref _isConfettiFlagEnabled, value))
            {
                _playerService.ToggleConfettiFlag(_isConfettiFlagEnabled);
                _playerService.ToggleInfiniteBuffs(_isConfettiFlagEnabled);
                if (!_isConfettiFlagEnabled)
                {
                    _playerService.ToggleInfiniteBuffs(_isGachiinFlagEnabled);
                }
            }
        }
    }

    private bool _isGachiinFlagEnabled;

    public bool IsGachiinFlagEnabled
    {
        get => _isGachiinFlagEnabled;
        set
        {
            if (SetProperty(ref _isGachiinFlagEnabled, value))
            {
                
                _playerService.ToggleGachiinFlag(_isGachiinFlagEnabled);
                _playerService.ToggleInfiniteBuffs(_isGachiinFlagEnabled);
                if (!_isGachiinFlagEnabled)
                {
                    _playerService.ToggleInfiniteBuffs(_isConfettiFlagEnabled);
                }
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

    private int _apChange;

    public int ApChange
    {
        get => _apChange;
        set
        {
            if (SetProperty(ref _apChange, value))
            {
                _playerService.SetAttackPower(_apChange);
            }
        }
    }

    private bool _isAutoSetNewGameSevenEnabled;
    
    public bool IsAutoSetNewGameSevenEnabled
    {
        get => _isAutoSetNewGameSevenEnabled;
        set => SetProperty(ref _isAutoSetNewGameSevenEnabled, value);
    }

    private int _currentHealth;

    public int CurrentHealth
    {
        get => _currentHealth;
        set => SetProperty(ref _currentHealth, value);
    }

    private int _maxHealth;

    public int MaxHealth
    {
        get => _maxHealth;
        set => SetProperty(ref _maxHealth, value);
    }

    private int _maxPosture;

    public int MaxPosture
    {
        get => _maxPosture;
        set => SetProperty(ref _maxPosture, value);
    }
    
    private int _currentPosture;

    public int CurrentPosture
    {
        get => _currentPosture;
        set => SetProperty(ref _currentPosture, value);
    }
    
    private int _requestRespawn;

    public int RequestRespawn
    {
        get => _requestRespawn;
        set => SetProperty(ref _requestRespawn, value);
    }
    
    
    private float _playerSpeed;
    
    public float PlayerSpeed
    {
        get => _playerSpeed;
        set
        {
            if (SetProperty(ref _playerSpeed, value))
            {
                _playerService.SetSpeed(_playerSpeed);
            }
        }
    }

    private int _currentAp;

    public int CurrentAp
    {
        get => _currentAp;
        set => SetProperty(ref _currentAp, value);
    }

    private int _currentExperience;

    public int CurrentExperience
    {
        get => _currentExperience;
        set => SetProperty(ref _currentExperience, value);
    }

    private int _addSenUpDown;

    public int AddSenUpDown
    {
       get => _addSenUpDown;
       set => SetProperty(ref _addSenUpDown, value);
    }
    
    private int _addExperienceUpDown;

    public int AddExperienceUpDown
    {
        get => _addExperienceUpDown;
        set => SetProperty(ref _addExperienceUpDown, value);
    }


    private float _damageMultiplier = DefaultDamageMultiplier;

    public float DamageMultiplier
    {
        get => _damageMultiplier;
        set
        {
            if (SetProperty(ref _damageMultiplier, value))
            {
                if (IsDamageMultiplierEnabled) _playerService.SetDamageMultiplier(_damageMultiplier);
            }
        }
    }

    private bool _isDamageMultiplierEnabled;

    public bool IsDamageMultiplierEnabled
    {
        get => _isDamageMultiplierEnabled;
        set
        {
            if (SetProperty(ref _isDamageMultiplierEnabled, value))
            {
                if (_isDamageMultiplierEnabled)
                {
                    _playerService.SetDamageMultiplier(_damageMultiplier);
                }
                _playerService.ToggleDamageMultiplier(_isDamageMultiplierEnabled);
            }
        }
    }
    
    
    public void SetSpeed(double value) => PlayerSpeed = (float)value;
    
    #endregion

    #region Public Methods

    public void PauseUpdates() => _pauseUpdates = true;
    public void ResumeUpdates() => _pauseUpdates = false;
    public void SetNewGame(int newGameCycle) => _playerService.SetNewGame(newGameCycle);
    public void SetHp(int health) => _playerService.SetHp(health);
    public void SetPosture(int posture) => _playerService.SetPosture(posture);
    public void SetAttackPower(int ap) => _playerService.SetAttackPower(ap);
    
    
    #endregion

    #region Private Methods

    private void RegisterHotkeys() 
    {
        _hotkeyManager.RegisterAction(HotkeyActions.SavePos1, () => SavePosition(0));
        _hotkeyManager.RegisterAction(HotkeyActions.SavePos2, () => SavePosition(1));
        _hotkeyManager.RegisterAction(HotkeyActions.RestorePos1, () => RestorePosition(0));
        _hotkeyManager.RegisterAction(HotkeyActions.RestorePos2, () => RestorePosition(1));
        _hotkeyManager.RegisterAction(HotkeyActions.ApplyConfetti, SetApplyConfetti);
        _hotkeyManager.RegisterAction(HotkeyActions.ApplyGachiin, SetApplyGachiin);
        _hotkeyManager.RegisterAction(HotkeyActions.RemoveConfetti, SetRemoveConfetti);
        _hotkeyManager.RegisterAction(HotkeyActions.RemoveGachiin, SetRemoveGachiin);
        _hotkeyManager.RegisterAction(HotkeyActions.NoDamage,() => { IsNoDamageEnabled = !IsNoDamageEnabled; }); //do this for toggles
        
        _hotkeyManager.RegisterAction(HotkeyActions.OneShotHealth, () => { IsOneShotEnabled = !IsOneShotEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.OneShotPosture, () => { IsOneShotPostureEnabled = !IsOneShotPostureEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.NoGoodsConsume, () => { IsNoGoodsConsumeEnabled = !IsNoGoodsConsumeEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.NoEmblemConsume, () => { IsNoEmblemConsumeEnabled = !IsNoEmblemConsumeEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.InfiniteRevival, () => { IsNoRevivalConsumeEnabled = !IsNoRevivalConsumeEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.PlayerHide, () => { IsPlayerHideEnabled = !IsPlayerHideEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.PlayerSilent, () => { IsPlayerSilentEnabled = !IsPlayerSilentEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.InfinitePoise, () => { IsInfinitePoiseEnabled = !IsInfinitePoiseEnabled; });
        

        _hotkeyManager.RegisterAction(HotkeyActions.NoDeath, () => { IsNoDeathEnabled = !IsNoDeathEnabled; });

        _hotkeyManager.RegisterAction(HotkeyActions.NoDeathExKillbox, () => { IsNoDeathEnabledWithoutKillbox = !IsNoDeathEnabledWithoutKillbox; });
        
        _hotkeyManager.RegisterAction(HotkeyActions.TogglePlayerSpeed, () => TogglePlayerSpeed());
        _hotkeyManager.RegisterAction(HotkeyActions.IncreasePlayerSpeed, () => SetSpeed(Math.Min(10, PlayerSpeed + 0.25f)));
        _hotkeyManager.RegisterAction(HotkeyActions.DecreasePlayerSpeed, () => SetSpeed(Math.Max(0, PlayerSpeed - 0.25f)));
        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseDamageMultiplier, () => DamageMultiplier = Math.Min(10, DamageMultiplier + 0.1f));
        _hotkeyManager.RegisterAction(HotkeyActions.DecreaseDamageMultiplier, () => DamageMultiplier = Math.Max(0.1f, PlayerSpeed - 0.1f));
        _hotkeyManager.RegisterAction(HotkeyActions.ToggleDamageMultiplier, () => IsDamageMultiplierEnabled = !IsDamageMultiplierEnabled);


    }
    
    

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        if (IsNoDeathEnabled) _playerService.TogglePlayerNoDeath(true); 
        
        if (IsNoDeathEnabledWithoutKillbox) _playerService.TogglePlayerNoDeathWithoutKillbox(true); 
        
        if (IsNoDamageEnabled) _playerService.TogglePlayerNoDamage(true);

        if (IsOneShotEnabled) _playerService.TogglePlayerOneShotHealth(true);

        if (IsOneShotPostureEnabled) _playerService.TogglePlayerOneShotPosture(true);

        if (IsNoGoodsConsumeEnabled) _playerService.TogglePlayerNoGoodsConsume(true);

        if (IsNoEmblemConsumeEnabled) _playerService.TogglePlayerNoGoodsConsume(true);

        if (IsNoRevivalConsumeEnabled) _playerService.TogglePlayerNoRevivalConsume(true);

        if (IsPlayerHideEnabled) _playerService.TogglePlayerHide(true);

        if (IsPlayerSilentEnabled) _playerService.TogglePlayerSilent(true);

        if (IsInfinitePoiseEnabled) _playerService.TogglePlayerInfinitePoise(true);
        _playerTick.Start();

        if (_isConfettiFlagEnabled)
        {
            _playerService.ToggleConfettiFlag(true);
            _playerService.ToggleInfiniteBuffs(true);
        }

        if (_isGachiinFlagEnabled)
        {
            _playerService.ToggleGachiinFlag(true);
            _playerService.ToggleInfiniteBuffs(true);
        }

        if (IsDamageMultiplierEnabled)
        {
            _playerService.SetDamageMultiplier(_damageMultiplier);
            _playerService.ToggleDamageMultiplier(true);
        }
        
        NewGame = _playerService.GetNewGame();
    }


    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        _playerTick.Stop();
    }
    
    private void OnGameStart()
    {
        if (!IsAutoSetNewGameSevenEnabled) return;
        _playerService.SetNewGame(7);
        NewGame = _playerService.GetNewGame();
    }

    private void PlayerTick(object? sender, EventArgs e)
    {
        if (_pauseUpdates) return;
        var coords = _playerService.GetCoords();
        PosX = coords.x;
        PosY = coords.y;
        PosZ = coords.z;
        
        CurrentHealth = _playerService.GetCurrentHp();
        MaxHealth = _playerService.GetMaxHp();
        CurrentPosture = _playerService.GetCurrentPosture();
        MaxPosture = _playerService.GetMaxPosture();
        PlayerSpeed =  _playerService.GetPlayerSpeed();
        CurrentAp = _playerService.GetAttackPower();
        // CurrentExperience = _playerService.GetExperience();

    }


    private void SavePosition(object parameter)
    {
        int index = Convert.ToInt32(parameter);
        if (index == 0) IsPos1Saved = true;
        else IsPos2Saved = true;

        _playerService.SavePos(index);
    }

    private void RestorePosition(object parameter)
    {
        int index = Convert.ToInt32(parameter);
        if (index == 0 && !IsPos1Saved) return;
        if (index == 1 && !IsPos2Saved) return;
        _playerService.RestorePos(index);
    
    }

    private void SetMaxHp()
    {
        _playerService.SetHp(MaxHealth);
    }

    private void SetMaxPosture()
    {
        _playerService.SetPosture(MaxPosture);
    }

    private void SetRemoveConfetti()
    {
        _playerService.RemoveSpecialEffect(SpecialEffect.Confetti);
    }

    private void SetRemoveGachiin()
    {
        _playerService.RemoveSpecialEffect(SpecialEffect.Gachiin);
    }
    

    private void SetApplyConfetti()
    {
        _playerService.ApplySpecialEffect(SpecialEffect.Confetti);
    }
    
    private void SetApplyGachiin()
    {
        _playerService.ApplySpecialEffect(SpecialEffect.Gachiin);
    }
    
    private void Rest()
    {
        _playerService.Rest();
    }

    private void SetAddSen()
    {
        _playerService.AddSen(AddSenUpDown);
    }
    
    private void SetAddExperience()
    {
        _playerService.AddExperience(AddExperienceUpDown);
    }
    
    private void TogglePlayerSpeed()
    {
        if (!AreOptionsEnabled) return;

        if (!IsApproximately(PlayerSpeed, DefaultSpeed))
        {
            _playerDesiredSpeed = _playerSpeed;
            SetSpeed(DefaultSpeed);
        }
        else if (_playerDesiredSpeed >= 0)
        {
            SetSpeed(_playerDesiredSpeed);
        }
    }
    private bool IsApproximately(float a, float b) => Math.Abs(a - b) < Epsilon;
    
    #endregion
}