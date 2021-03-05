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

namespace CharGraph.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private int _min1 = -12, _min2 = -12, _max1 = 24, _max2 = 24, _lines = 5, _resolution = 0, _nullpoint = 0;
        private bool _mode = false;
        private float _exp = 0;
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
        public ICommand ExpChanged { get; }
        public ICommand NullPointChanged { get; }
        public ICommand ResolutionChanged { get; }

        public SeriesCollection Series { get; set; } = new SeriesCollection();
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
            ExpChanged = new Command(() => Draw());
            NullPointChanged = new Command(() => Draw());
            ResolutionChanged = new Command(() => Draw());
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
        public int Lines
        {
            get => _lines;
            set => SetAndRaise(ref _lines, value);
        }
        public int Resolution
        {
            get => _resolution;
            set => SetAndRaise(ref _resolution, value);
        }
        public int NullPoint
        {
            get => _nullpoint;
            set => SetAndRaise(ref _nullpoint, value);
        }
        public bool Mode
        {
            get => _mode;
            set => SetAndRaise(ref _mode, value);
        }
        public float Exp
        {
            get => _exp;
            set => SetAndRaise(ref _exp, value);
        }

        public List<int> Fuses { get; } = new List<int>() { 100, 250, 500, 1000, 1500 };
        public List<int> Fuses2 { get; } = new List<int>() { 20, 50, 100, 200, 300 };

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

        private void Draw()
        {
            int multiplier = 10;
            switch (Resolution)
            {
                case (0):
                    multiplier = 10;
                    break;
                case (1):
                    multiplier = 25;
                    break;
                case (2):
                    multiplier = 50;
                    break;

            }

            Series.Clear();
            LineSeries line = new LineSeries();
            line.Title = "positive";
            line.PointGeometrySize = 0;
            ChartValues<ObservablePoint> chart = new LiveCharts.ChartValues<ObservablePoint>();
            for (int i = 0; i < 36; i++)
            {
                double y = multiplier * Math.Pow(1.00 / (1 + i), Exp);
                if (i + NullPoint <= 24)
                {
                    ObservablePoint point = new ObservablePoint(i + NullPoint, y);
                    chart.Add(point);
                }
            }
            line.Values = chart;
            Series.Add(line);

            LineSeries line2 = new LineSeries();
            line2.Title = "negative";
            line2.PointGeometrySize = 0;
            ChartValues<ObservablePoint> chart2 = new LiveCharts.ChartValues<ObservablePoint>();
            for (int i = 0; i < 36; i++)
            {
                double y = multiplier * Math.Pow(1.00 / (1 + i), Exp);
                if (-i + NullPoint >= -12)
                {
                    ObservablePoint point = new ObservablePoint(-i + NullPoint, y);
                    chart2.Add(point);
                }
            }
            line2.Values = chart2;
            Series.Add(line2);
        }

    }
}
