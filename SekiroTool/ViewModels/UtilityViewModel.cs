using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class UtilityViewModel : BaseViewModel
{
    private const float DefaultNoclipMultiplier = 1f;
    private const float DefaultGameSpeed = 1f;
    private const float Epsilon = 0.0001f;

    private readonly IUtilityService _utilityService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IDebugDrawService _debugDrawService;
    private readonly PlayerViewModel _playerViewModel;


    private bool _wasNoDeathEnabled;
    private float _desiredSpeed = 2;

    public UtilityViewModel(IUtilityService utilityService, IStateService stateService,
        HotkeyManager hotkeyManager, IDebugDrawService debugDrawService, PlayerViewModel playerViewModel)
    {
        _utilityService = utilityService;
        _hotkeyManager = hotkeyManager;
        _debugDrawService = debugDrawService;
        _playerViewModel = playerViewModel;

        RegisterHotkeys();

        stateService.Subscribe(State.Loaded, OnGameLoaded);
        stateService.Subscribe(State.NotLoaded, OnGameNotLoaded);
        stateService.Subscribe(State.Detached, OnGameDetached);


        MoveCamToPlayerCommand = new DelegateCommand(MoveCamToPlayer);

        OpenSkillsCommand = new DelegateCommand(OpenSkillMenu);
        OpenUpgradeProstheticsCommand = new DelegateCommand(OpenUpgradeProstheticsMenu);
        OpenUpgradePrayerBeadCommand = new DelegateCommand(OpenUpgradePrayerBead);

        IdolCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.Idol));
        CrowsBedMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.CrowsBedMemorialMob));
        BattlefieldMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.BattlefieldMemorialMob));
        AnayamaCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.Anayama));
        FujiokaCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.Fujioka));
        DungeonMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.DungeonMemorialMob));
        BadgerCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.Badger));
        ExiledMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.ExiledMemorialMob));
        ToxicMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.ToxicMemorialMob));
        ShugendoMemorialMobCommand = new DelegateCommand(() => OpenRegularShop(ShopLineup.ShugendoMemorialMob));
        HarunagaCommand = new DelegateCommand(() => OpenScalesShop(ScaleLineup.Harunaga));
        KoremoriCommand = new DelegateCommand(() => OpenScalesShop(ScaleLineup.Koremori));
        ProstheticsCommand = new DelegateCommand(() => OpenProstheticsShop(ShopLineup.Prosthetics));
    }


    #region Commands

    public ICommand MoveCamToPlayerCommand { get; set; }

    public ICommand OpenSkillsCommand { get; set; }
    public ICommand OpenUpgradeProstheticsCommand { get; set; }
    public ICommand OpenUpgradePrayerBeadCommand { get; set; }

    public ICommand IdolCommand { get; set; }
    public ICommand CrowsBedMemorialMobCommand { get; set; }
    public ICommand BattlefieldMemorialMobCommand { get; set; }
    public ICommand AnayamaCommand { get; set; }
    public ICommand FujiokaCommand { get; set; }
    public ICommand DungeonMemorialMobCommand { get; set; }
    public ICommand BadgerCommand { get; set; }
    public ICommand ExiledMemorialMobCommand { get; set; }
    public ICommand ToxicMemorialMobCommand { get; set; }
    public ICommand ShugendoMemorialMobCommand { get; set; }
    public ICommand HarunagaCommand { get; set; }
    public ICommand KoremoriCommand { get; set; }
    public ICommand ProstheticsCommand { get; set; }

    #endregion

    #region Properties

    private bool _areOptionsEnabled;

    public bool AreOptionsEnabled
    {
        get => _areOptionsEnabled;
        set => SetProperty(ref _areOptionsEnabled, value);
    }

    private bool _isHitboxViewEnabled;

    public bool IsHitBoxViewEnabled
    {
        get => _isHitboxViewEnabled;
        set
        {
            if (!SetProperty(ref _isHitboxViewEnabled, value)) return;
            if (_isHitboxViewEnabled) _debugDrawService.RequestDebugDraw();
            else _debugDrawService.ReleaseDebugDraw();
            _utilityService.ToggleHitboxView(_isHitboxViewEnabled);
        }
    }

    private bool _isSoundViewEnabled;

    public bool IsSoundViewEnabled
    {
        get => _isSoundViewEnabled;
        set
        {
            if (!SetProperty(ref _isSoundViewEnabled, value)) return;
            if (_isSoundViewEnabled) _debugDrawService.RequestDebugDraw();
            else _debugDrawService.ReleaseDebugDraw();
            _utilityService.TogglePlayerSoundView(_isSoundViewEnabled);
        }
    }

    private bool _isHideMapEnabled;

    public bool IsHideMapEnabled
    {
        get => _isHideMapEnabled;
        set
        {
            if (!SetProperty(ref _isHideMapEnabled, value)) return;
            _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowMap, _isHideMapEnabled);
        }
    }

    private bool _isHideObjEnabled;

    public bool IsHideObjEnabled
    {
        get => _isHideObjEnabled;
        set
        {
            if (!SetProperty(ref _isHideObjEnabled, value)) return;
            _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowObj, _isHideObjEnabled);
        }
    }

    private bool _isHideChrEnabled;

    public bool IsHideChrEnabled
    {
        get => _isHideChrEnabled;
        set
        {
            if (!SetProperty(ref _isHideChrEnabled, value)) return;
            _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowChr, _isHideChrEnabled);
        }
    }


    private bool _isHideSfxEnabled;

    public bool IsHideSfxEnabled
    {
        get => _isHideSfxEnabled;
        set
        {
            if (!SetProperty(ref _isHideSfxEnabled, value)) return;
            _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowSfx, _isHideSfxEnabled);
        }
    }

    private bool _isHideGrassEnabled;

    public bool IsHideGrassEnabled
    {
        get => _isHideGrassEnabled;
        set
        {
            if (!SetProperty(ref _isHideGrassEnabled, value)) return;
            _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowGrass, _isHideGrassEnabled);
        }
    }

    private bool _isDrawLowHitEnabled;

    public bool IsDrawLowHitEnabled
    {
        get => _isDrawLowHitEnabled;
        set
        {
            if (!SetProperty(ref _isDrawLowHitEnabled, value)) return;
            _utilityService.ToggleMeshFlag(Offsets.MeshBase.LowHit, _isDrawLowHitEnabled);
        }
    }

    private bool _isDrawHighHitEnabled;

    public bool IsDrawHighHitEnabled
    {
        get => _isDrawHighHitEnabled;
        set
        {
            if (!SetProperty(ref _isDrawHighHitEnabled, value)) return;
            _utilityService.ToggleMeshFlag(Offsets.MeshBase.HighHit, _isDrawHighHitEnabled);
        }
    }

    private bool _isDrawObjMeshEnabled;

    public bool IsDrawObjMeshEnabled
    {
        get => _isDrawObjMeshEnabled;
        set
        {
            if (!SetProperty(ref _isDrawObjMeshEnabled, value)) return;
            _utilityService.ToggleMeshFlag(Offsets.MeshBase.Objects, _isDrawObjMeshEnabled);
        }
    }

    private bool _isDrawChrRagdollEnabled;

    public bool IsDrawChrRagdollEnabled
    {
        get => _isDrawChrRagdollEnabled;
        set
        {
            if (!SetProperty(ref _isDrawChrRagdollEnabled, value)) return;
            if (_isDrawChrRagdollEnabled) _debugDrawService.RequestDebugDraw();
            else _debugDrawService.ReleaseDebugDraw();
            _utilityService.ToggleMeshFlag(Offsets.MeshBase.Chr, _isDrawChrRagdollEnabled);
        }
    }


    private float _gameSpeed;

    public float GameSpeed
    {
        get => _gameSpeed;
        set
        {
            if (SetProperty(ref _gameSpeed, value))
            {
                _utilityService.SetGameSpeed(_gameSpeed);
            }
        }
    }
    
    private bool _isSaveInCombatEnabled;

    public bool IsSaveInCombatEnabled
    {
        get => _isSaveInCombatEnabled;
        set
        {
            if (SetProperty(ref _isSaveInCombatEnabled, value))
            {
                _utilityService.ToggleSaveInCombat(_isSaveInCombatEnabled);
            }
        }
    }

    private float _noClipSpeedMultiplier = DefaultNoclipMultiplier;

    public float NoClipSpeed
    {
        get => _noClipSpeedMultiplier;
        set
        {
            if (SetProperty(ref _noClipSpeedMultiplier, value))
            {
                if (!IsNoClipEnabled) return;
                _utilityService.WriteNoClipSpeed(_noClipSpeedMultiplier);
            }
        }
    }

    private bool _isNoClipEnabled;

    public bool IsNoClipEnabled
    {
        get => _isNoClipEnabled;
        set
        {
            if (!SetProperty(ref _isNoClipEnabled, value)) return;

            if (_isNoClipEnabled)
            {
                _utilityService.WriteNoClipSpeed(NoClipSpeed);
                _utilityService.ToggleNoClip(_isNoClipEnabled);
                _wasNoDeathEnabled = _playerViewModel.IsNoDeathEnabled;
                _playerViewModel.IsNoDeathEnabled = true;
            }
            else
            {
                _utilityService.ToggleNoClip(_isNoClipEnabled);
                _playerViewModel.IsNoDeathEnabled = _wasNoDeathEnabled;
            }
        }
    }

    private bool _isFreeCamEnabled;

    public bool IsFreeCamEnabled
    {
        get => _isFreeCamEnabled;
        set
        {
            if (!SetProperty(ref _isFreeCamEnabled, value)) return;
            if (_isFreeCamEnabled)
            {
                IsNoClipEnabled = false;
                _utilityService.ToggleFreeCamera(true);
                _utilityService.SetCameraMode(FreeCamMode);
            }
            else
            {
                _utilityService.ToggleFreeCamera(false);
                _utilityService.SetCameraMode(2);
            }
        }
    }

    private int _freeCamMode = 1;

    public int FreeCamMode
    {
        get => _freeCamMode;
        set
        {
            if (SetProperty(ref _freeCamMode, value) && IsFreeCamEnabled)
            {
                _utilityService.SetCameraMode(value);
            }
        }
    }

    public bool IsFreeCamMode1Selected
    {
        get => _freeCamMode == 1;
        set
        {
            if (value) FreeCamMode = 1;
        }
    }

    public bool IsFreeCamMode2Selected
    {
        get => _freeCamMode == 2;
        set
        {
            if (value) FreeCamMode = 2;
        }
    }

    #endregion

    #region Public Methods

    public void SetNoClipSpeed(double value) => NoClipSpeed = (float)value;
    public void SetGameSpeed(double value) => GameSpeed = (float)value;

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.ToggleGameSpeed, ToggleGameSpeed);
        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed,
            () => GameSpeed = (Math.Min(10, GameSpeed + 0.50f)));
        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseGameSpeed,
            () => GameSpeed = (Math.Min(10, GameSpeed + 0.50f)));
        _hotkeyManager.RegisterAction(HotkeyActions.DecreaseGameSpeed,
            () => GameSpeed = (Math.Max(0, GameSpeed - 0.50f)));
        _hotkeyManager.RegisterAction(HotkeyActions.NoClip, () =>
        {
            if (!AreOptionsEnabled) return;
            IsNoClipEnabled = !IsNoClipEnabled;
        });
        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseNoClipSpeed, () =>
        {
            if (IsNoClipEnabled) NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
        });
        _hotkeyManager.RegisterAction(HotkeyActions.DecreaseNoClipSpeed, () =>
        {
            if (IsNoClipEnabled) NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
        });
        _hotkeyManager.RegisterAction(HotkeyActions.FreeCam, () =>
        {
            if (!AreOptionsEnabled) return;
            IsFreeCamEnabled = !IsFreeCamEnabled;
        });
        _hotkeyManager.RegisterAction(HotkeyActions.MoveCamToPlayer, () =>
        {
            if (!AreOptionsEnabled || !IsFreeCamEnabled) return;
            _utilityService.MoveCamToPlayer();
        });
        
        
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
        GameSpeed = _utilityService.GetGameSpeed();
        if (IsHitBoxViewEnabled) _utilityService.ToggleHitboxView(true);
        if (IsSoundViewEnabled) EnableDrawFeature(() => _utilityService.TogglePlayerSoundView(true));
        if (IsHideMapEnabled) _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowMap, true);
        if (IsHideObjEnabled) _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowObj, true);
        if (IsHideChrEnabled) _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowChr, true);
        if (IsHideSfxEnabled) _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowSfx, true);
        if (IsHideGrassEnabled) _utilityService.ToggleGameRendFlag(Offsets.GameRendFlags.ShowGrass, true);
        if (IsDrawLowHitEnabled) _utilityService.ToggleMeshFlag(Offsets.MeshBase.LowHit, true);
        if (IsDrawHighHitEnabled) _utilityService.ToggleMeshFlag(Offsets.MeshBase.HighHit, true);
        if (IsDrawObjMeshEnabled) _utilityService.ToggleMeshFlag(Offsets.MeshBase.Objects, true);
        if (IsDrawChrRagdollEnabled) EnableDrawFeature(() => _utilityService.ToggleMeshFlag(Offsets.MeshBase.Chr, true));
        if (IsSaveInCombatEnabled) _utilityService.ToggleSaveInCombat(true);
    }

    private void EnableDrawFeature(Action action)
    {
        if (_debugDrawService.GetCount() < 1) _debugDrawService.RequestDebugDraw();
        action.Invoke();
    }


    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        IsNoClipEnabled = false;
        GameSpeed = 1;
    }

    private void OnGameDetached()
    {
        _wasNoDeathEnabled = false;
    }

    private void ToggleGameSpeed()
    {
        if (!IsApproximately(GameSpeed, DefaultGameSpeed))
        {
            _desiredSpeed = GameSpeed;
            GameSpeed = DefaultGameSpeed;
        }
        else if (_desiredSpeed >= 0)
        {
            GameSpeed = _desiredSpeed;
        }
    }

    private bool IsApproximately(float a, float b) => Math.Abs(a - b) < Epsilon;

    private void MoveCamToPlayer() => _utilityService.MoveCamToPlayer();

    private void OpenSkillMenu() => _utilityService.OpenSkillMenu();
    private void OpenUpgradeProstheticsMenu() => _utilityService.OpenUpgradeProstheticsMenu();
    private void OpenRegularShop(ShopLineup shopLineup) => _utilityService.OpenRegularShop(shopLineup);
    private void OpenScalesShop(ScaleLineup scaleLineup) => _utilityService.OpenScalesShop(scaleLineup);
    private void OpenProstheticsShop(ShopLineup shopLineup) => _utilityService.OpenProstheticsShop(shopLineup);
    private void OpenUpgradePrayerBead() => _utilityService.OpenUpgradePrayerBead();

    #endregion
}