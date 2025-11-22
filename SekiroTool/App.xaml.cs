using System.Configuration;
using System.Data;
using System.Windows;

namespace SekiroTool;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static Mutex _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        const string appName = "SekiroTool";

        _mutex = new Mutex(true, appName, out var createdNew);

        if (!createdNew)
        {
            Current.Shutdown();
        }

        base.OnStartup(e);
    }       
}