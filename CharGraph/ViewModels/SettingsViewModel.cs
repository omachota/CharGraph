using System;
using System.Collections.Generic;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;

namespace CharGraph.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private int _min1 = -12, _min2 = -12, _max1 = 24, _max2 = 24;
		private int _fuse1Index, _fuse2Index;
		private bool _isArduinoDialogOpen;
		private readonly INavigator _navigator;
		private readonly ArduinoDetector _arduinoDetector;
		public Func<double, string> XFormatter { get; set; }
		public Func<double, string> YFormatter { get; set; }
		
		public ICommand AcceptArduinoDialogCommand { get; }
		public ICommand CancelArduinoDialogCommand { get; }
		public ICommand Min1ValueChanged { get; }
		public ICommand Min2ValueChanged { get; }
		public ICommand Max1ValueChanged { get; }
		public ICommand Max2ValueChanged { get; }
		public ICommand Fuse1Changed { get; }
		public ICommand Fuse2Changed { get; }


		public SettingsViewModel(INavigator navigator, ArduinoDetector arduinoDetector)
		{
			_navigator = navigator;
			_arduinoDetector = arduinoDetector;
			AcceptArduinoDialogCommand = new Command(AcceptArduinoDialog);
			CancelArduinoDialogCommand = new Command(() => IsArduinoDialogOpen = false);
			Min1ValueChanged = new Command(() => Write($"Min1 {-Min1}"));
			Min2ValueChanged = new Command(() => Write($"Min2 {-Min2}"));
			Max1ValueChanged = new Command(() => Write($"Max1 {Max1}"));
			Max2ValueChanged = new Command(() => Write($"Max2 {Max2}"));
			Fuse1Changed = new Command(() => Write($"Fuse1 {Fuses[Fuse1Index]}"));
			Fuse2Changed = new Command(() => Write($"Fuse2 {Fuses2[Fuse2Index]}"));
			ParseSettings();
		}

		private void ParseSettings()
		{
			var settings = Extensions.ReadSettings();
			Min1 = settings.Min1;
			Min2 = settings.Min2;
			Max1 = settings.Max1;
			Max2 = settings.Max2;
			Fuse1Index = settings.Fuse1;
			Fuse2Index = settings.Fuse2;
		}

		private bool IsArduinoDialogOpen
		{
			get => _isArduinoDialogOpen;
			set => SetAndRaise(ref _isArduinoDialogOpen, value);
		}

		public int Min1
		{
			get => _min1;
			set => SetAndRaise(ref _min1, value);
		}

		public int Min2
		{
			get => _min2;
			set => SetAndRaise(ref _min2, value);
		}

		public int Max1
		{
			get => _max1;
			set => SetAndRaise(ref _max1, value);
		}

		public int Max2
		{
			get => _max2;
			set => SetAndRaise(ref _max2, value);
		}

		public int Fuse1Index
		{
			get => _fuse1Index;
			set => SetAndRaise(ref _fuse1Index, value);
		}

		public int Fuse2Index
		{
			get => _fuse2Index;
			set => SetAndRaise(ref _fuse2Index, value);
		}

		public List<int> Fuses { get; } = new List<int>() {100, 250, 500, 1000, 1500};
		public List<int> Fuses2 { get; } = new List<int>() {20, 50, 100, 200, 300};

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
		}

		private void Write(string text)
		{
			if (_arduinoDetector.Arduino != null)
			{
				_arduinoDetector.Arduino.Write(text);
				Thread.Sleep(25);
			}

			Extensions.SaveSettings(_min1, _min2, _max1, _max2, _fuse1Index, _fuse2Index);
		}
	}
}
