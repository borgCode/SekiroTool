using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class EventTab : UserControl
{
    public EventTab(EventViewModel eventViewModel)
    {
        InitializeComponent();
        DataContext = eventViewModel;
    }
}