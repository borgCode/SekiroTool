using System.Windows.Controls;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class EnemyTab : UserControl
{

    private readonly EnemyViewModel _enemyViewModel;
    
    public EnemyTab(EnemyViewModel enemyViewModel)
    {
        _enemyViewModel = enemyViewModel;
        InitializeComponent();
        DataContext = enemyViewModel;

        InitializeUpDownHelpers();
    }

    private void InitializeUpDownHelpers()
    {
        new UpDownHelper<double>(
            SpeedUpDown,
            _enemyViewModel.SetSpeed
        );
    }
}