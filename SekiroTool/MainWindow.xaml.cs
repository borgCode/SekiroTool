using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using SekiroTool.Enums;
using SekiroTool.Interfaces;
using SekiroTool.Memory;
using SekiroTool.Services;

namespace SekiroTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IMemoryService _memoryService;
    private readonly IGameStateService _gameStateService;
    private readonly IPlayerService _playerService;
    private readonly IEnemyTargetService _enemyTargetService;

    private readonly AoBScanner _aobScanner;
    private readonly HookManager _hookManager;

    private readonly DispatcherTimer _gameLoadedTimer;

    public MainWindow()
    {
        _memoryService = new MemoryService();
        _memoryService.StartAutoAttach();

        InitializeComponent();

        _aobScanner = new AoBScanner(_memoryService);
        _hookManager = new HookManager(_memoryService);

        _gameStateService = new GameStateService(_memoryService);
        _playerService = new PlayerService(_memoryService);
        _enemyTargetService = new EnemyTargetService(_memoryService, _hookManager);

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

    private bool _hasAppliedNoLogo;

    private bool _appliedOneTimeFeatures;

    private bool _hasAppliedAttachedFeatures;

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

            if (!_hasAppliedNoLogo)
            {
                // _memoryIo.WriteBytes(Patches.NoLogo, AsmLoader.GetAsmBytes("NoLogo"));
                _hasAppliedNoLogo = true;
            }

            if (!_hasAppliedAttachedFeatures)
            {
                // _settingsViewModel.ApplyAttachedSettings();
                _hasAppliedAttachedFeatures = true;
            }

            if (!_hasAllocatedMemory)
            {
                _memoryService.AllocCodeCave();
                Console.WriteLine($"Code cave: 0x{CodeCaveOffsets.Base.ToInt64():X}");
                _hasAllocatedMemory = true;
                _gameStateService.Publish(GameState.Attached);
                Console.WriteLine("Attached");
            }
            

            if (_gameStateService.IsLoaded())
            {
                if (_loaded) return;
                _loaded = true;
                _gameStateService.Publish(GameState.Loaded);
                Console.WriteLine("Loaded");
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
                Console.WriteLine("Not loaded");
            }
        }
        else
        {
            if (_memoryService.IsAttached)
            {
                _gameStateService.Publish(GameState.Detached);
                Console.WriteLine("Detached");
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

    private void Test(object sender, RoutedEventArgs e)
    {
        // _enemyTargetService.ToggleNoDeath(true);
    }

    private void TestOff(object sender, RoutedEventArgs e)
    {
        // _enemyTargetService.ToggleNoDeath(false);
    }
}