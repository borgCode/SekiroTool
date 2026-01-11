using System.ComponentModel;
using System.IO;
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
using static SekiroTool.Memory.Offsets;

namespace SekiroTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MemoryService _memoryService;
    private readonly IStateService _stateService;
    private readonly IPlayerService _playerService;

    private readonly AoBScanner _aobScanner;
    private readonly HotkeyManager _hotkeyManager;
    private readonly NopManager _nopManager;

    private readonly DispatcherTimer _gameLoadedTimer;

    public MainWindow()
    {
        _memoryService = new MemoryService();
        _memoryService.StartAutoAttach();

        InitializeComponent();
        
        if (SettingsManager.Default.WindowLeft != 0 || SettingsManager.Default.WindowTop != 0)
        {
            Left = SettingsManager.Default.WindowLeft;
            Top = SettingsManager.Default.WindowTop;
        }
        else WindowStartupLocation = WindowStartupLocation.CenterScreen;

        _aobScanner = new AoBScanner(_memoryService);
        _stateService = new StateService(_memoryService);
        
        var hookManager = new HookManager(_memoryService, _stateService);
        
        _nopManager = new NopManager(_memoryService, _stateService);
        _hotkeyManager = new HotkeyManager(_memoryService);

        _playerService = new PlayerService(_memoryService, hookManager);
        IReminderService reminderService = new ReminderService(_memoryService);
        ITravelService travelService = new TravelService(_memoryService, hookManager);
        IEnemyService enemyService = new EnemyService(_memoryService, hookManager, reminderService);
        ITargetService targetService = new TargetService(_memoryService, hookManager, reminderService);
        IDebugDrawService debugDrawService = new DebugDrawService(_memoryService, _stateService, _nopManager);
        IEventService eventService = new EventService(_memoryService);
        IUtilityService utilityService = new UtilityService(_memoryService, hookManager);
        IItemService itemService = new ItemService(_memoryService);
        ISettingsService settingsService = new SettingsService(_memoryService, _nopManager, hookManager);
        IEzStateService ezStateService = new EzStateService(_memoryService);


        PlayerViewModel playerViewModel = new PlayerViewModel(_playerService, _hotkeyManager, _stateService);
        TravelViewModel travelViewModel =
            new TravelViewModel(travelService, _stateService, _hotkeyManager, eventService);
        EnemyViewModel enemyViewModel = new EnemyViewModel(enemyService, _hotkeyManager, _stateService,
            debugDrawService, eventService);
        TargetViewModel targetViewModel =
            new TargetViewModel(_stateService, _hotkeyManager, targetService, debugDrawService);
        UtilityViewModel utilityViewModel =
            new UtilityViewModel(utilityService, _stateService, _hotkeyManager, debugDrawService, playerViewModel, ezStateService);
        ItemViewModel itemViewModel = new ItemViewModel(itemService, _stateService);
        EventViewModel eventViewModel = new EventViewModel(eventService, _stateService, debugDrawService, itemService);
        SettingsViewModel settingsViewModel = new SettingsViewModel(settingsService, _stateService, _hotkeyManager);

        var playerTab = new PlayerTab(playerViewModel);
        var travelTab = new TravelTab(travelViewModel);
        var enemyTab = new EnemyTab(enemyViewModel);
        var targetTab = new TargetTab(targetViewModel);
        var utilityTab = new UtilityTab(utilityViewModel);
        var itemTab = new ItemTab(itemViewModel);
        var eventTab = new EventTab(eventViewModel);
        var settingsTab = new SettingsTab(settingsViewModel);

        MainTabControl.Items.Add(new TabItem { Header = "Player", Content = playerTab });
        MainTabControl.Items.Add(new TabItem { Header = "Travel", Content = travelTab });
        MainTabControl.Items.Add(new TabItem { Header = "Enemies", Content = enemyTab });
        MainTabControl.Items.Add(new TabItem { Header = "Target", Content = targetTab });
        MainTabControl.Items.Add(new TabItem { Header = "Utility", Content = utilityTab });
        MainTabControl.Items.Add(new TabItem { Header = "Items", Content = itemTab });
        MainTabControl.Items.Add(new TabItem { Header = "Event", Content = eventTab });
        MainTabControl.Items.Add(new TabItem { Header = "Settings", Content = settingsTab });
        
        MainTabControl.SelectionChanged += MainTabControl_SelectionChanged;

        settingsViewModel.ApplyStartUpOptions();
        
        Closing += MainWindow_Closing;

        _gameLoadedTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };
        _gameLoadedTimer.Tick += Timer_Tick;
        _gameLoadedTimer.Start();
        
        VersionChecker.UpdateVersionText(AppVersion);
        if (SettingsManager.Default.EnableUpdateChecks)
        {
            VersionChecker.CheckForUpdates(this);
        }
    }


    private bool _loaded;

    private bool _hasScanned;

    private bool _hasAllocatedMemory;
    
    private bool _hasPublishedAttached;
    private bool _hasCheckedPatch;

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (_memoryService.IsAttached)
        {
            IsAttachedText.Text = "Attached to game";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["AttachedBrush"];
            LaunchGameButton.IsEnabled = false;

            if (!_hasCheckedPatch)
            {
                if (!PatchChecker.Initialize(_memoryService))
                {
                    _aobScanner.DoEarlyScan();
                    _stateService.Publish(State.EarlyAttached);
                    _aobScanner.DoMainScan();
                }
                
#if DEBUG
                Console.WriteLine($@"Base: 0x{_memoryService.BaseAddress.ToInt64():X}");
#endif
                _hasCheckedPatch = true;
            }
            
            if (!_hasAllocatedMemory)
            {
                _memoryService.AllocCodeCave();
                Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
                _hasAllocatedMemory = true;
              
            }
            
            if (!_hasPublishedAttached)
            {
                _stateService.Publish(State.Attached);
                _hasPublishedAttached = true;
            }

            if (_stateService.IsLoaded())
            {
                if (_loaded) return;
                _loaded = true;
                _stateService.Publish(State.Loaded);
                TrySetGameStartPrefs();
            }
            else if (_loaded)
            {
                _stateService.Publish(State.NotLoaded);
                _loaded = false;
            }
        }
        else
        {
            if (_hasPublishedAttached)
            {
                _stateService.Publish(State.Detached);
                _hasPublishedAttached = false;
            }

            _loaded = false;
            _hasScanned = false;
            _hasAllocatedMemory = false;
            IsAttachedText.Text = "Not attached";
            IsAttachedText.Foreground = (SolidColorBrush)Application.Current.Resources["NotAttachedBrush"];
            LaunchGameButton.IsEnabled = true;
        }
    }

    private void TrySetGameStartPrefs()
    {
        var igt = _memoryService.ReadInt64((IntPtr)_memoryService.ReadInt64(GameDataMan.Base) + GameDataMan.IGT);
        if (igt < 5000) _stateService.Publish(State.GameStart);
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
        SettingsManager.Default.WindowLeft = Left;
        SettingsManager.Default.WindowTop = Top;
        SettingsManager.Default.Save();
    }
    
    private void LaunchGame_Click(object sender, RoutedEventArgs e) => Task.Run(GameLauncher.LaunchSekiro);
    private void CheckUpdate_Click(object sender, RoutedEventArgs e) => VersionChecker.CheckForUpdates(this, true);
    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is TabControl && MainTabControl.SelectedItem is TabItem selectedTab)
        {
            if (selectedTab.Header.ToString() == "Event")
            {
                _stateService.Publish(State.EventTabActivated);
            }
        }
    }
}