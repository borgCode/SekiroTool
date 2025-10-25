using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class TravelTab : UserControl
{
    public TravelTab(TravelViewModel travelViewModel)
    {
        InitializeComponent();
        DataContext = travelViewModel;
    }
}