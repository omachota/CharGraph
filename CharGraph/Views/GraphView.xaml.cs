using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using CharGraph.Models;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace CharGraph.Views
{
	public partial class GraphView : UserControl
	{
		public List<ArduData> ArduData { get; set; } = new List<ArduData>();

		public GraphView()
		{
			InitializeComponent();

			List<string> titles = new List<string>();

			using (StreamReader sr = new StreamReader(@"D:\Download\Dokument.csv"))
			{
				string line;
				string title = "";

				while ((line = sr.ReadLine()) != null)
				{
					if (line.Contains(";"))
					{
						ArduData.Add(new ArduData(line, title));
					}
					else if ("new line" == line)
					{
						title = sr.ReadLine();
						titles.Add(title);
					}
					else if ("Stop" == line)
					{
						break;
					}
				}
			}

			ArduData = ArduData.GroupBy(elem=>elem.X).Select(group=>group.First()).ToList();

			ArduData = ArduData.OrderBy(x => x.X).ToList();

			for (int i = 0; i < titles.Count; i++)
			{
				SeriesCollection.Add(new LineSeries
				{
					Title = titles[i],
				});

				var points = ArduData.Where(x => x.Title == titles[i]).ToList();

				SeriesCollection[SeriesCollection.Count - 1].Values = new ChartValues<ObservablePoint>();

				for (int j = 0; j < points.Count; j++)
				{
					SeriesCollection[SeriesCollection.Count - 1].Values.Add(new ObservablePoint(points[j].X, points[j].Y));
				}
			}

			// SeriesCollection = new SeriesCollection
			// {
			// 	new LineSeries
			// 	{
			// 		Title = "Series 1",
			// 		Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
			// 	},
			// 	new LineSeries
			// 	{
			// 		Title = "Series 2",
			// 		Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },
			// 		PointGeometry = null
			// 	},
			// 	new LineSeries
			// 	{
			// 		Title = "Series 3",
			// 		Values = new ChartValues<double> { 4,2,7,2,7 },
			// 		PointGeometry = DefaultGeometries.Square,
			// 		PointGeometrySize = 15
			// 	}
			// };
			//
			// Labels = new[] {"Jan", "Feb", "Mar", "Apr", "May"};
			// YFormatter = value => value.ToString("C");
			//
			// //modifying the series collection will animate and update the chart
			// SeriesCollection.Add(new LineSeries
			// {
			// 	Title = "Series 4",
			// 	Values = new ChartValues<double> {5, 3, 2, 4},
			// 	LineSmoothness = 0, //0: straight lines, 1: really smooth lines
			// 	PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
			// 	PointGeometrySize = 50,
			// 	PointForeground = Brushes.Gray
			// });
			//
			// //modifying any series values will also animate and update the chart
			// SeriesCollection[3].Values.Add(5d);

			DataContext = this;
		}

		public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }
	}
}
