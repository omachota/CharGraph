using System.Windows;
using CharGraph.ViewModels;

namespace CharGraph
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(MainViewModel viewModel)
		{
			DataContext = viewModel;

			InitializeComponent();
		}
	}
}