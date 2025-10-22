using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
using SekiroTool.GameIds;
using SekiroTool.Interfaces;
using SekiroTool.Utilities;

namespace SekiroTool.ViewModels;

public class UtilityViewModel : BaseViewModel
{
    private readonly IUtilityService _utilityService;
    private readonly HotkeyManager _hotkeyManager;
    private readonly IDebugDrawService _debugDrawService;

    public UtilityViewModel(IUtilityService utilityService, IGameStateService gameStateService,
        HotkeyManager hotkeyManager, IDebugDrawService debugDrawService)
    {
        _utilityService = utilityService;
        _hotkeyManager = hotkeyManager;
        _debugDrawService = debugDrawService;
        
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
    #endregion
    
    #region Private Methods

    private void RegisterHotkeys()
    {
    }
    
    private void OnGameLoaded()
    {
        AreOptionsEnabled = true;
    }
    
    
    private void OnGameNotLoaded()
    {
        AreOptionsEnabled = false;
    }

    private void OpenSkillMenu() => _utilityService.OpenSkillMenu();
    private void OpenUpgradeProstheticsMenu() => _utilityService.OpenUpgradeProstheticsMenu();
    private void OpenRegularShop(ShopLineup shopLineup) => _utilityService.OpenRegularShop(shopLineup);
    private void OpenScalesShop(ScaleLineup scaleLineup) => _utilityService.OpenScalesShop(scaleLineup);
    private void OpenProstheticsShop(ShopLineup shopLineup) => _utilityService.OpenProstheticsShop(shopLineup);

    #endregion
}