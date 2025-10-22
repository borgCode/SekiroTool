using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class UtilityTab : UserControl
{
    public UtilityTab(UtilityViewModel utilityViewModel)
    {
        InitializeComponent();
        DataContext = utilityViewModel;
    }
}