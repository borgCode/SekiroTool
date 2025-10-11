using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SekiroTool.Interfaces;
using SekiroTool.Services;

namespace SekiroTool;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    
    private readonly IMemoryService _memoryService;
    
    public MainWindow()
    {
        _memoryService = new MemoryService();
        _memoryService.StartAutoAttach();
        
        InitializeComponent();
    }
}