using System;
using System.Collections.Generic;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CharGraph.Models;
using CharGraph.Views;

namespace CharGraph.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private string _test;
		private readonly INavigator _navigator;
		private bool _isArduinoDialogOpen;
		private object _arduinoDialogObject;
		private List<string> _comPorts;
		private string _selectedItem;
		private Arduino Arduino { get; set; }

		public ICommand OpenArduinoDialogCommand { get; }
		public ICommand AcceptArduinoDialogCommand { get; }
		public ICommand CancelArduinoDialogCommand { get; }

		public SettingsViewModel(INavigator navigator)
		{
			_navigator = navigator;
			AcceptArduinoDialogCommand = new Command(AcceptArduinoDialog);
			CancelArduinoDialogCommand = new Command(() => IsArduinoDialogOpen = false);
			OpenArduinoDialogCommand = new Command(OpenArduinoDialog);
			Task.Run(ArduinoDetector.GetArduinoPorts).ContinueWith(t => ComPorts = t.Result);
			Task.Run(async () =>
			{
				for (int i = 0; i < 10; i++)
				{
					Test = $"Settings {i}";
					await Task.Delay(1000).ConfigureAwait(false);
				}
			});
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

		public List<string> ComPorts
		{
			get => _comPorts;
			set => SetAndRaise(ref _comPorts, value);
		}

		public string SelectedItem
		{
			get => _selectedItem;
			set => SetAndRaise(ref _selectedItem, value);
		}

		private void OpenArduinoDialog()
		{
			ArduinoDialogObject = new ArduinoDetectedDialog();
			IsArduinoDialogOpen = true;
		}

		private void AcceptArduinoDialog()
		{
			IsArduinoDialogOpen = false;
			Task.Delay(TimeSpan.FromSeconds(0.5)).ContinueWith(t => _navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Main),
				TaskScheduler.FromCurrentSynchronizationContext());
		}

		public async Task Initialize(CancellationTokenSource cts)
		{
			await Task.Run(() =>
			{
				Arduino = ArduinoDetector.GetArduino();
				while (Arduino == null && !cts.IsCancellationRequested)
				{
					Arduino = ArduinoDetector.GetArduino();
					Thread.Sleep(1000);
				}
			}, cts.Token);

			if (Arduino.Name != null)
				OpenArduinoDialog();
		}

		public string Test
		{
			get => _test;
			set => SetAndRaise(ref _test, value);
		}
	}
}
