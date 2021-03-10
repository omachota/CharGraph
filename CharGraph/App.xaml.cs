using System.IO;
using System.Windows;
using CharGraph.Infrastructure;
using CharGraph.Models;
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
            if (!Directory.Exists(Extensions.Directory))
                Directory.CreateDirectory(Extensions.Directory);

            if (!File.Exists(Extensions.FilePath))
                Settings.SaveDefault();

            Window window = new MainWindow(new MainViewModel());

            window.Show();

            base.OnStartup(e);
        }
    }
}
