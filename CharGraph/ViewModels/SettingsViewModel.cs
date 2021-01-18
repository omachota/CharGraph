using System;
using System.Collections.ObjectModel;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CharGraph.Views;

namespace CharGraph.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private bool _isArduinoDialogOpen;
		private object _arduinoDialogObject;
		private string _selectedPort;
		private readonly INavigator _navigator;
		private readonly ArduinoDetector _arduinoDetector;

		public ICommand OpenArduinoDialogCommand { get; }
		public ICommand AcceptArduinoDialogCommand { get; }
		public ICommand CancelArduinoDialogCommand { get; }

		public SettingsViewModel(INavigator navigator, ArduinoDetector arduinoDetector)
		{
			_navigator = navigator;
			_arduinoDetector = arduinoDetector;
			AcceptArduinoDialogCommand = new Command(AcceptArduinoDialog);
			CancelArduinoDialogCommand = new Command(() => IsArduinoDialogOpen = false);
			OpenArduinoDialogCommand = new Command(OpenArduinoDialog);
			// refresh comports until a ComPort is detected
			/*Task.Run(async () =>
			{
				for (int i = 0; i < 10; i++)
				{
					Test = $"Settings {i}";
					await Task.Delay(1000).ConfigureAwait(false);
				}
			});*/
		}

		public bool IsArduinoDialogOpen
		{
			get => _isArduinoDialogOpen;
			set => SetAndRaise(ref _isArduinoDialogOpen, value);
		}

		public object ArduinoDialogObject
		{
			get => _arduinoDialogObject;
			set => SetAndRaise(ref _arduinoDialogObject, value);
		}

		public ObservableCollection<string> ComPorts { get; set; }

		public string SelectedPort
		{
			get => _selectedPort;
			set => SetAndRaise(ref _selectedPort, value);
		}

		private void OpenArduinoDialog()
		{
			ArduinoDialogObject = new ArduinoDetectedDialog();
			IsArduinoDialogOpen = true;
		}

		private void AcceptArduinoDialog()
		{
			IsArduinoDialogOpen = false;
			Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(_ => _navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Main),
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		public async Task Initialize(CancellationTokenSource cts)
		{
			await Task.Run(async () =>
			{
				_arduinoDetector.InitializeArduino = true;
				while (_arduinoDetector.Arduino == null && !cts.IsCancellationRequested)
				{
					await Task.Delay(2000);
				}
			}, cts.Token);
			OpenArduinoDialog();
		}
	}
}
