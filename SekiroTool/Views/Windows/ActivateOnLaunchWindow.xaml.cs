using System.Windows;
using System.Windows.Input;
using SekiroTool.ViewModels;

namespace SekiroTool.Views.Windows;

public partial class ActivateOnLaunchWindow : Window
{
    public ActivateOnLaunchWindow(ActivateOnLaunchViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
