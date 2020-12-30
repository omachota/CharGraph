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
		public List<ArduData> ArduData { get; } = new List<ArduData>();

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
			DataContext = this;
		}

		public SeriesCollection SeriesCollection { get; } = new SeriesCollection();
	}
}
