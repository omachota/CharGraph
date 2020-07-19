using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;

namespace CharGraph.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private bool _isErrorMessageVisible;
		private string _errorMessage;
		private Visibility _isSettingsButtonVisible = Visibility.Collapsed;
		private Visibility _isMainButtonVisible = Visibility.Visible;
		public INavigator Navigator { get; }

		public ICommand SwitchViewCommand { get; }

		public MainViewModel()
		{
			Navigator = new Navigator();
			Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Settings);
			Navigator.OnCurrentWindowTypeChanged += NavigatorOnOnCurrentWindowTypeChanged;
			SwitchViewCommand = new Command(() =>
			{
				Navigator.UpdateCurrentViewModelCommand.Execute(Navigator.CurrentWindowType == ViewType.Main
					? ViewType.Settings
					: ViewType.Main);
			});
			Task.Run(ArduinoDetector.Checker);
			ArduinoDetector.ArduinoDisconnectedEvent = OnArduinoDisconnected;
			ArduinoDetector.ArduinoDetectedEvent = OnArduinoDetected;
		}

		private void NavigatorOnOnCurrentWindowTypeChanged(object sender, EventArgs e)
		{
			if (Navigator.CurrentWindowType == ViewType.Main)
			{
				IsSettingsButtonVisible = Visibility.Collapsed;
				IsMainButtonVisible = Visibility.Visible;
			}
			else
			{
				IsSettingsButtonVisible = Visibility.Visible;
				IsMainButtonVisible = Visibility.Collapsed;
			}
		}

		public bool IsErrorMessageVisible
		{
			get => _isErrorMessageVisible;
			set => SetAndRaise(ref _isErrorMessageVisible, value);
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set => SetAndRaise(ref _errorMessage, value);
		}

		public Visibility IsSettingsButtonVisible
		{
			get => _isSettingsButtonVisible;
			set => SetAndRaise(ref _isSettingsButtonVisible, value);
		}

		public Visibility IsMainButtonVisible
		{
			get => _isMainButtonVisible;
			set => SetAndRaise(ref _isMainButtonVisible, value);
		}

		private async void OnArduinoDisconnected()
		{
			Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Settings);
			ErrorMessage = "Arduino bylo odpojeno";
			IsErrorMessageVisible = true;
			await Task.Delay(3500).ConfigureAwait(false);
			IsErrorMessageVisible = false;
		}

		private async void OnArduinoDetected(string portName)
		{
			ErrorMessage = "Arduino nalezeno na portu: " + portName;
			IsErrorMessageVisible = true;
			await Task.Delay(3500).ConfigureAwait(false);
			IsErrorMessageVisible = false;
		}
	}
}
