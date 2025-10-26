using System.Windows.Controls;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class ItemTab : UserControl
{
    public ItemTab(ItemViewModel itemViewModel)
    {
        InitializeComponent();
        DataContext = itemViewModel;
    }
}