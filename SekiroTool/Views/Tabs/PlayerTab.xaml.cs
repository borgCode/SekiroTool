using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class PlayerTab : UserControl
{
    public PlayerTab(PlayerViewModel playerViewModel)
    {
        InitializeComponent();
        DataContext = playerViewModel;
    }
}