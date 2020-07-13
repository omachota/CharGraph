using System;
using System.Windows.Input;
using CharGraph.ViewModels;

namespace CharGraph.Infrastructure.SwitchView
{
	public class UpdateCurrentViewModelCommand : ICommand
	{
		public event EventHandler CanExecuteChanged = null!;

		private readonly INavigator _navigator;

		public UpdateCurrentViewModelCommand(INavigator navigator)
		{
			_navigator = navigator;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			var viewType = (ViewType) parameter;

			if (_navigator.CurrentWindowType != viewType)
			{
				_navigator.CurrentViewModel = viewType switch
				{
					ViewType.Main => new GraphViewModel(),
					ViewType.Settings => new SettingsViewModel(),
					_ => throw new ArgumentOutOfRangeException()
				};
				_navigator.CurrentWindowType = viewType;
			}
		}
	}
}
