using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class UtilityViewModel : BaseViewModel
{
    private const float DefaultNoclipMultiplier = 1f;
    
    private readonly IUtilityService _utilityService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IDebugDrawService _debugDrawService;
    private readonly PlayerViewModel _playerViewModel;


    private bool _wasNoDeathEnabled;

    public UtilityViewModel(IUtilityService utilityService, IGameStateService gameStateService,
        HotkeyManager hotkeyManager, IDebugDrawService debugDrawService, PlayerViewModel playerViewModel)
    {
        _utilityService = utilityService;
        _hotkeyManager = hotkeyManager;
        _debugDrawService = debugDrawService;
        _playerViewModel = playerViewModel;

        RegisterHotkeys();

        gameStateService.Subscribe(GameState.Loaded, OnGameLoaded);
        gameStateService.Subscribe(GameState.NotLoaded, OnGameNotLoaded);


        OpenSkillsCommand = new DelegateCommand(OpenSkillMenu);
        OpenUpgradeProstheticsCommand = new DelegateCommand(OpenUpgradeProstheticsMenu);

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

    public ICommand OpenSkillsCommand { get; set; }
    public ICommand OpenUpgradeProstheticsCommand { get; set; }

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

    #endregion

    #region Public Methods

    public void SetNoClipSpeed(double value) => NoClipSpeed = (float) value;

    #endregion

    #region Private Methods

    private void RegisterHotkeys()
    {
        _hotkeyManager.RegisterAction(HotkeyActions.NoClip.ToString(), () =>
            {
                if (!AreOptionsEnabled) return;
                IsNoClipEnabled = !IsNoClipEnabled;
            });
        _hotkeyManager.RegisterAction(HotkeyActions.IncreaseNoClipSpeed.ToString(), () =>
        {
            if (IsNoClipEnabled) NoClipSpeed = Math.Min(5, NoClipSpeed + 0.50f);
        });
        _hotkeyManager.RegisterAction(HotkeyActions.DecreaseNoClipSpeed.ToString(), () =>
        {
            if (IsNoClipEnabled) NoClipSpeed = Math.Max(0.05f, NoClipSpeed - 0.50f);
        });
    }

    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }


    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
        IsNoClipEnabled = false;
    }

    private void OpenSkillMenu() => _utilityService.OpenSkillMenu();
    private void OpenUpgradeProstheticsMenu() => _utilityService.OpenUpgradeProstheticsMenu();
    private void OpenRegularShop(ShopLineup shopLineup) => _utilityService.OpenRegularShop(shopLineup);
    private void OpenScalesShop(ScaleLineup scaleLineup) => _utilityService.OpenScalesShop(scaleLineup);
    private void OpenProstheticsShop(ShopLineup shopLineup) => _utilityService.OpenProstheticsShop(shopLineup);

    #endregion

    
}