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
		private bool _isMessageVisible;
		private string _message;
		private Visibility _isSettingsButtonVisible = Visibility.Collapsed;
		private Visibility _isMainButtonVisible = Visibility.Visible;
		private ArduinoDetector ArduinoDetector { get; }

		public INavigator Navigator { get; }
		public ICommand SwitchViewCommand { get; }

		public MainViewModel()
		{
			ArduinoDetector = new ArduinoDetector();
			Navigator = new Navigator(ArduinoDetector);
			Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Main);
			Navigator.OnCurrentWindowTypeChanged += NavigatorOnOnCurrentWindowTypeChanged;
			SwitchViewCommand = new Command(() =>
			{
				Navigator.UpdateCurrentViewModelCommand.Execute(Navigator.CurrentWindowType == ViewType.Main
					? ViewType.Settings
					: ViewType.Main);
			});
			ArduinoDetector.ArduinoDisconnectedEvent = OnArduinoDisconnected;
			ArduinoDetector.ArduinoDetectedEvent = OnArduinoDetected;
			Task.Run(() => ArduinoDetector.ScanArduinoPorts(2000)).ConfigureAwait(false);
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

		public bool IsMessageVisible
		{
			get => _isMessageVisible;
			set => SetAndRaise(ref _isMessageVisible, value);
		}

		public string Message
		{
			get => _message;
			set => SetAndRaise(ref _message, value);
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

		private async Task OnArduinoDisconnected()
		{
			
			Message = "Arduino bylo odpojeno";
			IsMessageVisible = true;
			await Task.Delay(3500).ConfigureAwait(false);
			IsMessageVisible = false;
		}

		private async Task OnArduinoDetected(string portName)
		{
			Message = "Arduino nalezeno na portu: " + portName;
			IsMessageVisible = true;
			await Task.Delay(3500).ConfigureAwait(false);
			IsMessageVisible = false;
		}
    }
}
