using System.Windows.Input;
using SekiroTool.Core;
using SekiroTool.Enums;
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
    }
    
    
    #region Commands
    
    public ICommand OpenSkillsCommand { get; set; }
    public ICommand OpenUpgradeProstheticsCommand { get; set; }

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

    #endregion
}