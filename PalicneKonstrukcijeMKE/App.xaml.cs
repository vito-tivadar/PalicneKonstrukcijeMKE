using PalicneKonstrukcijeMKE.CustomControls;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System.Diagnostics;
using System.Windows;

namespace PalicneKonstrukcijeMKE;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static ProgramMessageBlock MessageBlock;
    public TrussMainViewModel trussMainViewModel { get; }

    public App()
    {
        trussMainViewModel = new TrussMainViewModel();
        
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow()
        {
            DataContext = this,
        };
        MainWindow.Show();

        base.OnStartup(e);
    }

}


