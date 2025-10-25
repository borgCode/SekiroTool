using System.Windows.Controls;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class TargetTab : UserControl
{

    private readonly TargetViewModel _targetViewModel;
    
    public TargetTab(TargetViewModel targetViewModel)
    {
        _targetViewModel = targetViewModel;
        InitializeComponent();
        DataContext = targetViewModel;

        InitializeUpDownHelpers();
    }

    private void InitializeUpDownHelpers()
    {
        _ = new UpDownHelper<double>(
            SpeedUpDown,
            _targetViewModel.SetSpeed
        );
    }
}