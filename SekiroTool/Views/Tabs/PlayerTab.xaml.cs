using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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

    private void DamageMultiplierTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var textBox = (TextBox)sender;
            BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty)?.UpdateSource();
        }
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

         _ = new UpDownHelper<int>(
             PostureUpDown,
             _playerViewModel.SetPosture,
             _playerViewModel.PauseUpdates,
             _playerViewModel.ResumeUpdates
         );
         
         _ = new UpDownHelper<int>(
             ApUpDown,
             _playerViewModel.SetAttackPower,
             _playerViewModel.PauseUpdates,
             _playerViewModel.ResumeUpdates
         );
        
         
        
    }   
      
}