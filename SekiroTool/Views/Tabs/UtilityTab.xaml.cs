using System.Windows.Controls;
using SekiroTool.Utilities;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Tabs;

public partial class UtilityTab : UserControl
{
    private readonly UtilityViewModel _utilityViewModel;

    public UtilityTab(UtilityViewModel utilityViewModel)
    {
        _utilityViewModel = utilityViewModel;
        InitializeComponent();
        DataContext = _utilityViewModel;

        InitializeUpDownHelpers();
    }

    private void InitializeUpDownHelpers()
    {
        _ = new UpDownHelper<double>(
            GameSpeedUpDown,
            _utilityViewModel.SetGameSpeed
        );
        _ = new UpDownHelper<double>(
            NoClipSpeedUpDown,
            _utilityViewModel.SetNoClipSpeed
        );
    }
}