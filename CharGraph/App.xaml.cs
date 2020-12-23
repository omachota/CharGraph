using System.Windows;
using CharGraph.Infrastructure;
using CharGraph.ViewModels;

namespace CharGraph
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Window window = new MainWindow(new MainViewModel());

            // ThemeSwitcher.ApplyBase(true);

            window.Show();

            base.OnStartup(e);
        }
    }
}
