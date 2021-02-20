﻿using System;
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
		public Func<double, string> XFormatter { get; set; }
		public Func<double, string> YFormatter { get; set; }
		private readonly ArduinoDetector _arduinoDetector;
		private ZoomingOptions _zoomingMode;
		private List<ArduData> DataList { get; set; } = new List<ArduData>();
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
				List<string> titles = new List<string>();
				_arduinoDetector.Arduino.ReadEvent += incomingData =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
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
									if(points[j].X < 1 || points[j].Y > 0)
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
