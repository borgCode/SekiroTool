using System.Windows.Input;
using System.Windows.Threading;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class TargetViewModel : BaseViewModel
{
    private readonly HotkeyManager _hotkeyManager;
    private readonly ITargetService _targetService;
    private readonly IDebugDrawService _debugDrawService;
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

    private int _forceAct;
    private int _lastAct;
    private int _forceKengekiAct;
    private int _lastKengekiAct;
    private bool _isRepeatActEnabled;
    private bool _isRepeatKengekiActEnabled;

    private float _targetSpeed;
    private float _targetDesiredSpeed = -1f;
    private const float DefaultSpeed = 1f;
    private const float Epsilon = 0.0001f;

    private bool _isAiFreezEnabled;
    private bool _isNoAttackEnabled;
    private bool _isNoMoveEnabled;
    private bool _isNoDeathEnabled;
    private bool _isNoPostureBuildupEnabled;
    private bool _isTargetViewEnabled;

    public TargetViewModel(IStateService stateService, HotkeyManager hotkeyManager,
        ITargetService targetService, IDebugDrawService debugDrawService)
    {
        _hotkeyManager = hotkeyManager;
        _targetService = targetService;
        _debugDrawService = debugDrawService;

        RegisterHotkeys();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);

        SetHpCommand = new DelegateCommand(SetHp);
        SetHpPercentageCommand = new DelegateCommand(SetHpPercentage);
        SetCustomHpCommand = new DelegateCommand(SetCustomHp);

        SetPostureCommand = new DelegateCommand(SetPosture);
        SetPosturePercentageCommand = new DelegateCommand(SetPosturePercentage);
        SetCustomPostureCommand = new DelegateCommand(SetCustomPosture);

        _targetTick = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(64)
        };
        _targetTick.Tick += TargetTick;
    }

    #region Commands

    public ICommand SetHpCommand { get; set; }
    public ICommand SetHpPercentageCommand { get; set; }
    public ICommand SetCustomHpCommand { get; set; }

    public ICommand SetPostureCommand { get; set; }
    public ICommand SetPosturePercentageCommand { get; set; }
    public ICommand SetCustomPostureCommand { get; set; }

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
                _targetService.ToggleTargetHook(true);
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
                _targetService.ToggleTargetHook(false);
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
            _targetService.ToggleNoDamage(_isFreezeHealthEnabled);
        }
    }

    public int CustomPosture
    {
        get => _customPosture;
        set
        {
            if (SetProperty(ref _customPosture, value))
            {
                _customPostureHasBeenSet = true;
            }
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
            _targetService.ToggleFreezePosture(_isFreezePostureEnabled);
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
            _targetService.ForceAct(_forceAct);
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
            _targetService.ForceKengekiAct(_forceKengekiAct);
            if (_forceKengekiAct == 0) IsRepeatKengekiActEnabled = false;
        }
    }

    public bool IsRepeatActEnabled
    {
        get => _isRepeatActEnabled;
        set
        {
            if (!SetProperty(ref _isRepeatActEnabled, value)) return;

            bool isRepeating = _targetService.IsTargetRepeating();

            switch (value)
            {
                case true when !isRepeating:
                    _targetService.ToggleTargetRepeatAct(true);
                    ForceAct = _targetService.GetLastAct();
                    break;
                case false when isRepeating:
                    _targetService.ToggleTargetRepeatAct(false);
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

            bool isRepeating = _targetService.IsTargetRepeatingKengeki();

            switch (value)
            {
                case true when !isRepeating:
                    _targetService.ToggleTargetRepeatKengekiAct(true);
                    ForceKengekiAct = _targetService.GetLastKengekiAct();
                    break;
                case false when isRepeating:
                    _targetService.ToggleTargetRepeatKengekiAct(false);
                    ForceKengekiAct = 0;
                    break;
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
                _targetService.SetSpeed(value);
            }
        }
    }

    public void SetSpeed(double value) => TargetSpeed = (float)value;

    public bool IsAiFreezeEnabled
    {
        get => _isAiFreezEnabled;
        set
        {
            if (SetProperty(ref _isAiFreezEnabled, value))
            {
                _targetService.ToggleAiFreeze(_isAiFreezEnabled);
            }
        }
    }

    public bool IsNoAttackEnabled
    {
        get => _isNoAttackEnabled;
        set
        {
            if (SetProperty(ref _isNoAttackEnabled, value))
            {
                _targetService.ToggleNoAttack(_isNoAttackEnabled);
            }
        }
    }

    public bool IsNoMoveEnabled
    {
        get => _isNoMoveEnabled;
        set
        {
            if (SetProperty(ref _isNoMoveEnabled, value))
            {
                _targetService.ToggleNoMove(_isNoMoveEnabled);
            }
        }
    }

    public bool IsNoDeathEnabled
    {
        get => _isNoDeathEnabled;
        set
        {
            if (SetProperty(ref _isNoDeathEnabled, value))
            {
                _targetService.ToggleNoDeath(_isNoDeathEnabled);
            }
        }
    }

    public bool IsNoPostureBuildupEnabled
    {
        get => _isNoPostureBuildupEnabled;
        set
        {
            if (SetProperty(ref _isNoPostureBuildupEnabled, value))
            {
                _targetService.ToggleNoPostureBuildup(_isNoPostureBuildupEnabled);
            }
        }
    }

    public bool IsTargetViewEnabled
    {
        get => _isTargetViewEnabled;
        set
        {
            if (SetProperty(ref _isTargetViewEnabled, value))
            {
                if (_isTargetViewEnabled) _debugDrawService.RequestDebugDraw();
                else _debugDrawService.ReleaseDebugDraw();

                _targetService.ToggleTargetView(_isTargetViewEnabled);
            }
        }
    }

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.EnableTargetOptions,
            () => { IsTargetOptionsEnabled = !IsTargetOptionsEnabled; });
        _hotkeyManager.RegisterAction(HotkeyActions.FreezeTargetHp, () =>
            ExecuteTargetAction(() => IsFreezeHealthEnabled = !IsFreezeHealthEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.SetTargetOneHp, () =>
            ExecuteTargetAction(() => SetHp(1)));
        _hotkeyManager.RegisterAction(HotkeyActions.TargetCustomHp, () => ExecuteTargetAction(SetCustomHp));
        _hotkeyManager.RegisterAction(HotkeyActions.FreezeTargetPosture,
            () => ExecuteTargetAction(() => IsFreezePostureEnabled = !IsFreezePostureEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.SetTargetOnePosture,
            () => ExecuteTargetAction(() => SetPosture(1)));
        _hotkeyManager.RegisterAction(HotkeyActions.TargetCustomPosture,
            () => ExecuteTargetAction(SetCustomPosture));
        _hotkeyManager.RegisterAction(HotkeyActions.ShowAllResistances, () =>
        {
            if (!IsTargetOptionsEnabled) IsTargetOptionsEnabled = true;
            _showAllResistances = !_showAllResistances;
            UpdateResistancesDisplay();
        });
        _hotkeyManager.RegisterAction(HotkeyActions.RepeatAct,
            () => ExecuteTargetAction(() => IsRepeatActEnabled = !IsRepeatActEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.RepeatKengekiAct,
            () => ExecuteTargetAction(() => IsRepeatKengekiActEnabled = !IsRepeatKengekiActEnabled));

        _hotkeyManager.RegisterAction(HotkeyActions.IncrementForceAct, () =>
            ExecuteTargetAction(() =>
            {
                if (ForceAct + 1 > 99) ForceAct = 0;
                else ForceAct += 1;
            }));


        _hotkeyManager.RegisterAction(HotkeyActions.DecrementForceAct, () =>
            ExecuteTargetAction(() =>
            {
                if (ForceAct - 1 < 0) ForceAct = 99;
                else ForceAct -= 1;
            }));

        _hotkeyManager.RegisterAction(HotkeyActions.IncrementForceKengekiAct, () =>
            ExecuteTargetAction(() =>
            {
                if (ForceKengekiAct + 1 > 99) ForceKengekiAct = 0;
                else ForceKengekiAct += 1;
            }));

        _hotkeyManager.RegisterAction(HotkeyActions.DecrementForceKengekiAct, () =>
            ExecuteTargetAction(() =>
            {
                if (ForceKengekiAct - 1 < 0) ForceKengekiAct = 99;
                else ForceKengekiAct -= 1;
            }));


        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseTargetSpeed, () =>
            ExecuteTargetAction(() => SetSpeed(Math.Min(5, TargetSpeed + 0.25f))));
        _hotkeyManager.RegisterAction(HotkeyActions.DecreaseTargetSpeed, () =>
            ExecuteTargetAction(() => SetSpeed(Math.Max(0, TargetSpeed - 0.25f))));
        _hotkeyManager.RegisterAction(HotkeyActions.ToggleTargetSpeed, () =>
            ExecuteTargetAction(ToggleTargetSpeed));
        _hotkeyManager.RegisterAction(HotkeyActions.FreezeTargetAi,
            () => ExecuteTargetAction(() => IsAiFreezeEnabled = !IsAiFreezeEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.NoAttackTargetAi,
            () => ExecuteTargetAction(() => IsNoAttackEnabled = !IsNoAttackEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.NoMoveTargetAi,
            () => ExecuteTargetAction(() => IsNoMoveEnabled = !IsNoMoveEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.TargetNoPostureBuildup,
            () => ExecuteTargetAction(() => IsNoPostureBuildupEnabled = !IsNoPostureBuildupEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.TargetNoDeath,
            () => ExecuteTargetAction(() => IsNoDeathEnabled = !IsNoDeathEnabled));
        _hotkeyManager.RegisterAction(HotkeyActions.TargetTargetingView,
            () => ExecuteTargetAction(() => IsTargetViewEnabled = !IsTargetViewEnabled));
    }

    private void ExecuteTargetAction(Action action)
    {
        if (!IsTargetOptionsEnabled)
        {
            IsTargetOptionsEnabled = true;
            Task.Run(async () =>
            {
                await Task.Delay(100);
                if (EnsureValidTarget()) action();
            });
            return;
        }

        if (!IsValidTarget) return;
        action();
    }

    private bool EnsureValidTarget() => IsValidTarget || IsTargetValid();

    private void TargetTick(object? sender, EventArgs e)
    {
        if (!IsTargetValid())
        {
            IsValidTarget = false;
            return;
        }

        IsValidTarget = true;


        ulong targetAddr = _targetService.GetTargetChrIns();


        if (targetAddr != _currentTargetAddr)
        {
            _currentTargetAddr = targetAddr;

#if DEBUG

            uint handle = _targetService.GetTargetHandle();
            uint characterId = _targetService.GetCharacterId();
            int entityId = _targetService.GetEntityId();
            Console.WriteLine($@"Target Info: handle: {handle:X} characterId: {characterId} entityId: {entityId} enemyIns: {targetAddr:X}");

#endif

            TargetMaxPoise = _targetService.GetMaxPoise();
            TargetMaxPoison = _targetService.GetMaxPoison();
            TargetMaxBurn = _targetService.GetMaxBurn();
            TargetMaxShock = _targetService.GetMaxShock();

            IsAiFreezeEnabled = _targetService.IsAiFreezeEnabled();
            IsNoAttackEnabled = _targetService.IsNoAttackEnabled();
            IsNoMoveEnabled = _targetService.IsNoMoveEnabled();
            IsFreezeHealthEnabled = _targetService.IsNoDamageEnabled();
            IsNoDeathEnabled = _targetService.IsNoDeathEnabled();
            IsNoPostureBuildupEnabled = _targetService.IsNoPostureBuildupEnabled();

            _isTargetViewEnabled = _targetService.IsTargetViewEnabled();
            OnPropertyChanged(nameof(IsTargetViewEnabled));
        }

        TargetMaxHealth = _targetService.GetMaxHp();
        TargetMaxPosture = _targetService.GetMaxPosture();
        TargetCurrentHealth = _targetService.GetCurrentHp();
        TargetCurrentPosture = _targetService.GetCurrentPosture();
        TargetCurrentPoise = _targetService.GetCurrentPoise();
        TargetPoiseTimer = _targetService.GetPoiseTimer();
        TargetCurrentPoison = _targetService.GetCurrentPoison();
        TargetCurrentBurn = _targetService.GetCurrentBurn();
        TargetCurrentShock = _targetService.GetCurrentShock();

        TargetSpeed = _targetService.GetSpeed();

        LastAct = _targetService.GetLastAct();
        LastKengekiAct = _targetService.GetLastKengekiAct();
    }

    private bool IsTargetValid()
    {
        ulong targetAddr = _targetService.GetTargetChrIns();
        if (targetAddr == 0) return false;

        float health = _targetService.GetCurrentHp();
        float maxHealth = _targetService.GetMaxHp();
        if (health < 0 || maxHealth <= 0 || health > 10000000 || maxHealth > 10000000) return false;

        if (health > maxHealth * 1.5) return false;

        var position = _targetService.GetPosition();

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
        IsFreezePostureEnabled = false;
        AreOptionsEnabled = false;
    }

    private void SetHp(object parameter) =>
        _targetService.SetHp(Convert.ToInt32(parameter));

    private void SetHpPercentage(object parameter)
    {
        int healthPercentage = Convert.ToInt32(parameter);
        int newHealth = TargetMaxHealth * healthPercentage / 100;
        _targetService.SetHp(newHealth);
    }

    private void SetCustomHp()
    {
        if (!_customHpHasBeenSet) return;
        if (CustomHp > TargetMaxHealth) CustomHp = TargetMaxHealth;
        _targetService.SetHp(CustomHp);
    }

    private void SetPosture(object parameter) =>
        _targetService.SetPosture(Convert.ToInt32(parameter));

    private void SetPosturePercentage(object parameter)
    {
        int posturePercentage = Convert.ToInt32(parameter);
        int newPosture = TargetMaxPosture * posturePercentage / 100;
        _targetService.SetPosture(newPosture);
    }

    private void SetCustomPosture()
    {
        if (!_customPostureHasBeenSet) return;
        if (CustomPosture > TargetMaxPosture) CustomPosture = TargetMaxPosture;
        _targetService.SetPosture(CustomPosture);
    }

    private void ToggleTargetSpeed()
    {
        if (!AreOptionsEnabled) return;

        if (!IsApproximately(TargetSpeed, DefaultSpeed))
        {
            _targetDesiredSpeed = TargetSpeed;
            SetSpeed(DefaultSpeed);
        }
        else if (_targetDesiredSpeed >= 0)
        {
            SetSpeed(_targetDesiredSpeed);
        }
    }

    private bool IsApproximately(float a, float b) => Math.Abs(a - b) < Epsilon;

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