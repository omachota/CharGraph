using System.Windows.Input;
using CharGraph.ViewModels;

namespace CharGraph.Infrastructure.SwitchView
{
	public enum ViewType
	{
		Main,
		Settings
	}

	public interface INavigator
	{
		BaseViewModel CurrentViewModel { get; set; }

		ICommand UpdateCurrentViewModelCommand { get; }

		ViewType CurrentWindowType { get; set; }
	}
}
