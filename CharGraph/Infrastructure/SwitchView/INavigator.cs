using System;
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

		event EventHandler OnCurrentWindowTypeChanged;

		ViewType CurrentWindowType { get; set; }
	}
}
