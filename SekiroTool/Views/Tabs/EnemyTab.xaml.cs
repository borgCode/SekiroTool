using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class EnemyTab : UserControl
{
    public EnemyTab(EnemyViewModel enemyViewModel)
    {
        InitializeComponent();
        DataContext = enemyViewModel;
    }
}