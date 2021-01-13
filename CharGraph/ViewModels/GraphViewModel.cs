using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CharGraph.Infrastructure;
using CharGraph.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace CharGraph.ViewModels
{
	public class GraphViewModel : BaseViewModel
	{
		private string _test;
		public Func<double, string> XFormatter { get; set; }
		public Func<double, string> YFormatter { get; set; }
		private ArduinoDetector _arduinoDetector { get; }
		private ZoomingOptions _zoomingMode;
		private List<ArduData> DataList { get; set; } = new List<ArduData>();
		public Command Start { get; }

		public GraphViewModel(ArduinoDetector arduinoDetector)
		{
			_arduinoDetector = arduinoDetector;
			Test = "AHOJ";
			Start = new Command(OnStart);
			ZoomingMode = ZoomingOptions.Y;
		}

		public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

		public ZoomingOptions ZoomingMode
		{
			get => _zoomingMode;
			set => SetAndRaise(ref _zoomingMode, value);
		}

		public string Test
		{
			get => _test;
			set => SetAndRaise(ref _test, value);
		}

		private void OnStart()
		{
			if (_arduinoDetector.Arduino != null)
			{
				string title = "";
				DataList.Clear();

				if (SeriesCollection.Count > 0)
				{
					SeriesCollection.RemoveAt(0);
					SeriesCollection.RemoveAt(0);
				}
				List<string> titles = new List<string>();
				_arduinoDetector.Arduino.ReadEvent += incommingData =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						if (incommingData.Contains(";"))
						{
							DataList.Add(new ArduData(incommingData, titles.Last()));
						}
						else if (incommingData.Contains("|"))
						{
							string[] datas = incommingData.Split('|');
							titles.Add(datas[1]);
						}
						else if (incommingData.Contains("Stop"))
						{
							DataList = DataList.GroupBy(elem => elem.X).Select(group => group.First()).ToList();

							DataList = DataList.OrderBy(x => x.X).ToList();
							SeriesCollection.Clear();
							for (int i = 0; i < titles.Count; i++)
							{
								SeriesCollection.Add(new LineSeries
								{
									Title = titles[i],
								});

								var points = DataList.Where(x => x.Title == titles[i]).ToList();

								SeriesCollection[SeriesCollection.Count - 1].Values = new ChartValues<ObservablePoint>();

								for (int j = 0; j < points.Count; j++)
								{
									SeriesCollection[SeriesCollection.Count - 1].Values.Add(new ObservablePoint(points[j].X, points[j].Y));
								}
							}
							titles.Clear();
							_arduinoDetector.Arduino.Flush();
						}

					});
				};
				_arduinoDetector.Arduino.Write("init");

			}
		}
	}
}
