﻿using System;
using System.Threading;
using System.Windows.Input;
using CharGraph.ViewModels;

namespace CharGraph.Infrastructure.SwitchView
{
	public class UpdateCurrentViewModelCommand : ICommand
	{
		private bool _showArduinoDetectedDialog = true;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
		private readonly INavigator _navigator;

		public event EventHandler CanExecuteChanged;

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
				switch (viewType)
				{
					case ViewType.Main:
						_navigator.CurrentViewModel = new GraphViewModel();
						_cts.Cancel();
						break;
					case ViewType.Settings:
						Settings();
						break;
					default:
						throw new ArgumentOutOfRangeException($"{nameof(Execute)}	{nameof(parameter)}");
				}

				_navigator.CurrentWindowType = viewType;
			}
		}

		private void Settings()
		{
			var settingsViewModel = new SettingsViewModel(_navigator);
			_navigator.CurrentViewModel = settingsViewModel;
			if (_showArduinoDetectedDialog)
			{
				_showArduinoDetectedDialog = false;
				settingsViewModel.Initialize(_cts);
			}

		}
	}
}
