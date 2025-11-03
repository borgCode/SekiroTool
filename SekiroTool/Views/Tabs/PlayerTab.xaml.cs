using System.Windows.Controls;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class PlayerTab : UserControl
{
    private readonly PlayerViewModel _playerViewModel;
    public PlayerTab(PlayerViewModel playerViewModel)
    {
        _playerViewModel = playerViewModel;
        InitializeComponent();
        DataContext = playerViewModel;

        InitializeUpDownHelpers();
    }

    private void InitializeUpDownHelpers()
    {
        _ = new UpDownHelper<int>(
            NewGameUpDown,
            _playerViewModel.SetNewGame,
            _playerViewModel.PauseUpdates,
            _playerViewModel.ResumeUpdates
        );
        _ = new UpDownHelper<int>(
            HealthUpDown,
            _playerViewModel.SetHp,
            _playerViewModel.PauseUpdates,
            _playerViewModel.ResumeUpdates
        );
                
            
    }   
}