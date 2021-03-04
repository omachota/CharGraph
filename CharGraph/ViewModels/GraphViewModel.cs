using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using CharGraph.Infrastructure;
using CharGraph.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace CharGraph.ViewModels
{
	public class GraphViewModel : BaseViewModel
	{
		private bool _isMeasuringEnabled;
		private bool _isButtonEnabled = true;
		private ZoomingOptions _zoomingMode;
		private readonly ArduinoDetector _arduinoDetector;
		private List<ArduData> DataList { get; set; } = new List<ArduData>();

		public Func<double, string> XFormatter { get; set; }
		public Func<double, string> YFormatter { get; set; }
		public Command Start { get; }

		public GraphViewModel(ArduinoDetector arduinoDetector)
		{
			_arduinoDetector = arduinoDetector;
			Start = new Command(OnStart);
			ZoomingMode = ZoomingOptions.Y;
		}

		public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

		public ZoomingOptions ZoomingMode
		{
			get => _zoomingMode;
			set => SetAndRaise(ref _zoomingMode, value);
		}

		public bool IsMeasuringEnabled
		{
			get => _isMeasuringEnabled;
			private set => SetAndRaise(ref _isMeasuringEnabled, value);
		}

		public bool IsButtonEnabled
		{
			get => _isButtonEnabled;
			private set => SetAndRaise(ref _isButtonEnabled, value);
		}

		private void OnStart()
		{
			if (_arduinoDetector.Arduino != null)
			{
				DataList.Clear();

				if (SeriesCollection.Count > 0)
				{
					SeriesCollection.RemoveAt(0);
					SeriesCollection.RemoveAt(0);
				}

				_arduinoDetector.Arduino.ReadEvent += ParseData;
				_arduinoDetector.Arduino.Write("init");
			}
		}

		private Task _progressBarTask;
		private CancellationTokenSource _progressBarCancellationToken;

		private void ParseData(string incomingData)
		{
			_progressBarCancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(3));
			if (_progressBarTask == null)
			{
				IsMeasuringEnabled = true;
				IsButtonEnabled = false;
				_progressBarTask = new Task(async () =>
				{
					while (!_progressBarCancellationToken.IsCancellationRequested)
					{
						await Task.Delay(1000).ConfigureAwait(false);
					}

					IsMeasuringEnabled = false;
					IsButtonEnabled = true;
					_progressBarTask = null;
				});
				_progressBarTask.Start();
			}
			List<string> titles = new List<string>();
			if (incomingData.Contains(";"))
			{
				DataList.Add(new ArduData(incomingData, titles.Last()));
			}
			else if (incomingData.Contains("|"))
			{
				string[] datas = incomingData.Split('|');
				titles.Add(datas[1]);
			}
			else if (incomingData.Contains("Stop"))
			{
				DataList = DataList.GroupBy(elem => elem.X).Select(group => @group.First()).ToList();

				DataList = DataList.OrderBy(x => x.X).ToList();
				Dispatcher.CurrentDispatcher.Invoke(() =>
				{
					SeriesCollection.Clear();
					for (int i = 0; i < titles.Count; i++)
					{
						SeriesCollection.Add(new LineSeries
						{
							Fill = Brushes.Transparent,
							Title = titles[i],
						});

						var points = DataList.Where(x => x.Title == titles[i]).ToList();

						SeriesCollection[SeriesCollection.Count - 1].Values = new ChartValues<ObservablePoint>();

						for (int j = 0; j < points.Count; j++)
						{
							//if(points[j].X < 1 || points[j].Y > 0)
							SeriesCollection[SeriesCollection.Count - 1].Values.Add(new ObservablePoint(points[j].X, points[j].Y));
						}
					}
				});

				_progressBarCancellationToken?.Cancel();

				titles.Clear();
				_arduinoDetector.Arduino.Flush();
			}
		}
	}
}
