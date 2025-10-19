using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Services;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;
using SekiroTool.Views.Tabs;

namespace SekiroTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IMemoryService _memoryService;
    private readonly IGameStateService _gameStateService;
    private readonly IPlayerService _playerService;

    private readonly AoBScanner _aobScanner;
    private readonly HookManager _hookManager;
    private readonly HotkeyManager _hotkeyManager;

    private readonly DispatcherTimer _gameLoadedTimer;

    public MainWindow()
    {
        _memoryService = new MemoryService();
        _memoryService.StartAutoAttach();

        InitializeComponent();

        _aobScanner = new AoBScanner(_memoryService);
        _hookManager = new HookManager(_memoryService);
        _hotkeyManager = new HotkeyManager(_memoryService);

        _gameStateService = new GameStateService(_memoryService);
        _playerService = new PlayerService(_memoryService);
        ITargetService targetService = new TargetService(_memoryService, _hookManager);
        IDebugDrawService debugDrawService = new DebugDrawService(_memoryService);
        IEventService eventService = new EventService(_memoryService);
        IItemService itemService = new ItemService(_memoryService);
        ISettingsService settingsService = new SettingsService(_memoryService);
        


        PlayerViewModel playerViewModel = new PlayerViewModel(_playerService, _hotkeyManager, _gameStateService);
        TargetViewModel targetViewModel =
            new TargetViewModel(_gameStateService, _hotkeyManager, targetService, debugDrawService);
        EventViewModel eventViewModel = new EventViewModel(eventService, _gameStateService, debugDrawService);
        SettingsViewModel settingsViewModel = new SettingsViewModel(settingsService, _gameStateService, _hotkeyManager);


        var playerTab = new PlayerTab(playerViewModel);
        var targetTab = new TargetTab(targetViewModel);
        var eventTab = new EventTab(eventViewModel);
        var settingsTab = new SettingsTab(settingsViewModel);
        
        MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
        MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
        MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
        MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });
        
        settingsViewModel.ApplyStartUpOptions();
        
        _gameLoadedTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };
        _gameLoadedTimer.Tick += Timer_Tick;
        _gameLoadedTimer.Start();
    }


    private bool _loaded;

    private bool _hasScanned;

    private bool _hasAllocatedMemory;
    

    private bool _appliedOneTimeFeatures;
    

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (_memoryService.IsAttached)
        {
            IsAttachedText.Text = "Attached to game";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];
            // LaunchGameButton.IsEnabled = false;

            if (!_hasScanned)
            {
                _aobScanner.Scan();
                _hasScanned = true;
            }
            
            if (!_hasAllocatedMemory)
            {
                _memoryService.AllocCodeCave();
                Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
                _hasAllocatedMemory = true;
                _gameStateService.Publish(GameState.Attached);
            
            }


            if (_gameStateService.IsLoaded())
            {
                if (_loaded) return;
                _loaded = true;
                _gameStateService.Publish(GameState.Loaded);
            
                // TryEnableFeatures();
                // TrySetGameStartPrefs();
                // if (_appliedOneTimeFeatures) return;
                // ApplyOneTimeFeatures();
                // _appliedOneTimeFeatures = true;
            }
            else if (_loaded)
            {
                _gameStateService.Publish(GameState.NotLoaded);
                // DisableFeatures();
                // _debugDrawService.Reset();
                _loaded = false;
             
            }
        }
        else
        {
            if (_memoryService.IsAttached)
            {
                _gameStateService.Publish(GameState.Detached);
        
            }

            _loaded = false;
            _hasScanned = false;
            _hasAllocatedMemory = false;

            // _hookManager.ClearHooks();
            // DisableFeatures();
            // _settingsViewModel.ResetLoaded();
            // _settingsViewModel.ResetAttached();
            // _nopManager.ClearRegistry();
            // _loaded = false;
            // _hasAllocatedMemory = false;
            // _hasAppliedNoLogo = false;
            // _appliedOneTimeFeatures = false;
            // _hasAppliedAttachedFeatures = false;
            IsAttachedText.Text = "Not attached";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
            // LaunchGameButton.IsEnabled = true;
        }
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        else
        {
            DragMove();
        }
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // SettingsManager.Default.WindowLeft = Left;
        // SettingsManager.Default.WindowTop = Top;
        // SettingsManager.Default.Save();
        // DisableFeatures();
        // _hookManager.UninstallAllHooks();
    }
}