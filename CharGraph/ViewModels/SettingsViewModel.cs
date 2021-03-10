using System;
using System.Collections.Generic;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Windows.Media;
using CharGraph.Models;

namespace CharGraph.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private int _minimum = -12, _maximum = 24;
		private double _tick = 1;
		private bool _isArduinoDialogOpen;
		private readonly INavigator _navigator;
		private readonly ArduinoDetector _arduinoDetector;
		private string _bazemin, _bazemax;
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
		public ICommand ExpChanged { get; }
		public ICommand NullPointChanged { get; }
		public ICommand ResolutionChanged { get; }
		public ICommand ModeChanged { get; }

		public SeriesCollection Series { get; set; } = new();

		public SettingsViewModel(INavigator navigator, ArduinoDetector arduinoDetector)
		{
			_navigator = navigator;
			_arduinoDetector = arduinoDetector;
			Settings = Extensions.ReadSettings();
			AcceptArduinoDialogCommand = new Command(AcceptArduinoDialog);
			CancelArduinoDialogCommand = new Command(() => IsArduinoDialogOpen = false);
			Min1ValueChanged = new Command(() => Write($"Min1 {-Settings.Min1}"));
			Min2ValueChanged = new Command<double>(_ =>
			{
				Write($"Min2 {-Settings.Min2}");
				Update();
			});
			Max1ValueChanged = new Command(() => Write($"Max1 {Settings.Max1}"));
			Max2ValueChanged = new Command<double>(_ =>
			{
				Write($"Max2 {Settings.Max2}");
				Update();
			});
			Fuse1Changed = new Command(() => Write($"Fuse1 {Fuses[Settings.Fuse1Index]}"));
			Fuse2Changed = new Command(() => Write($"Fuse2 {Fuses2[Settings.Fuse2Index]}"));
			ExpChanged = new Command(() =>
			{
				Draw();
				Settings.Save();
			});
			NullPointChanged = new Command(() =>
			{
				Draw();
				Settings.Save();
			});
			ResolutionChanged = new Command(() =>
			{
				Draw();
				Settings.Save();
			});
			ModeChanged = new Command(EventResetMode);
			Draw();
			Update();
		}

		public Settings Settings { get; private set; }

		private void Update()
		{
			string tmpmin = Settings.Min2.ToString();
			string tmpmax = Settings.Max2.ToString();

			if (Settings.Mode)
			{
				tmpmin += " mA";
				tmpmax += " mA";
			}
			else
			{
				tmpmin += " V";
				tmpmax += " V";
			}

			BazeMax = tmpmax;
			BazeMin = tmpmin;
		}

		private void EventResetMode()
		{
			if (!Settings.Mode)
			{
				Minimum = -10;
				Maximum = 20;
				Tick = 1;
				Settings.Min2 /= 12;
				Settings.Max2 /= 12;
			}
			else
			{
				Minimum = -120;
				Maximum = 240;
				Tick = 1;
				Settings.Min2 *= 12;
				Settings.Max2 *= 12;
			}

			Update();
		}

		private bool IsArduinoDialogOpen
		{
			get => _isArduinoDialogOpen;
			set => SetAndRaise(ref _isArduinoDialogOpen, value);
		}

		public int Minimum
		{
			get => _minimum;
			set => SetAndRaise(ref _minimum, value);
		}

		public int Maximum
		{
			get => _maximum;
			set => SetAndRaise(ref _maximum, value);
		}

		public double Tick
		{
			get => _tick;
			set => SetAndRaise(ref _tick, value);
		}

		public string BazeMax
		{
			get => _bazemax;
			set => SetAndRaise(ref _bazemax, value);
		}

		public string BazeMin
		{
			get => _bazemin;
			set => SetAndRaise(ref _bazemin, value);
		}

		public List<int> Fuses { get; } = new() {100, 250, 500, 1000, 1500};
		public List<int> Fuses2 { get; } = new() {20, 50, 100, 200, 300};

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

			Settings.Save();
		}

		private void Draw()
		{
			int multiplier = Settings.Resolution switch
			{
				0 => 10,
				1 => 25,
				2 => 50,
				_ => 10
			};

			Series.Clear();
			LineSeries line = new LineSeries();
			line.Title = "P";
			line.PointGeometrySize = 0;
			line.Fill = Brushes.Transparent;
			line.Stroke = new SolidColorBrush(Color.FromRgb(0xc6, 0xff, 0x00));
			ChartValues<ObservablePoint> chart = new ChartValues<ObservablePoint>();
			for (int i = 0; i < 30; i++)
			{
				double y = multiplier * Math.Pow(1.00 / (1 + i), Settings.Exp);
				if (i + Settings.NullPoint <= 20)
				{
					ObservablePoint point = new ObservablePoint(i + Settings.NullPoint, y);
					chart.Add(point);
				}
			}

			line.Values = chart;
			Series.Add(line);

			LineSeries line2 = new LineSeries();
			line2.Title = "N";
			line2.PointGeometrySize = 0;
			line2.Fill = Brushes.Transparent;
			line2.Stroke = new SolidColorBrush(Color.FromRgb(0xc6, 0xff, 0x00));
			ChartValues<ObservablePoint> chart2 = new ChartValues<ObservablePoint>();
			for (int i = 0; i < 30; i++)
			{
				double y = multiplier * Math.Pow(1.00 / (1 + i), Settings.Exp);
				if (-i + Settings.NullPoint >= -10)
				{
					ObservablePoint point = new ObservablePoint(-i + Settings.NullPoint, y);
					chart2.Add(point);
				}
			}

			line2.Values = chart2;
			Series.Add(line2);
		}
	}
}
