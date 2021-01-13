using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CharGraph.ViewModels;
using LiveCharts;

namespace CharGraph.Views
{
	public partial class GraphView : UserControl
	{
		private GraphViewModel _viewModel;
		public GraphView()
		{
			InitializeComponent();
		}

		private void ToogleZoomingMode(object sender, RoutedEventArgs e)
		{
			_viewModel = (GraphViewModel)DataContext;

			switch (_viewModel.ZoomingMode)
			{
				case ZoomingOptions.None:
					_viewModel.ZoomingMode = ZoomingOptions.X;
					break;
				case ZoomingOptions.X:
					_viewModel.ZoomingMode = ZoomingOptions.Y;
					break;
				case ZoomingOptions.Y:
					_viewModel.ZoomingMode = ZoomingOptions.Xy;
					break;
				case ZoomingOptions.Xy:
					_viewModel.ZoomingMode = ZoomingOptions.None;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ResetZoomOnClick(object sender, RoutedEventArgs e)
		{
			//Use the axis MinValue/MaxValue properties to specify the values to display.
			//use double.Nan to clear it.

			X.MinValue = double.NaN;
			X.MaxValue = double.NaN;
			Y.MinValue = double.NaN;
			Y.MaxValue = double.NaN;
		}
	}

	public class ZoomingModeCoverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			switch ((ZoomingOptions) value)
			{
				case ZoomingOptions.None:
					return "None";
				case ZoomingOptions.X:
					return "X";
				case ZoomingOptions.Y:
					return "Y";
				case ZoomingOptions.Xy:
					return "XY";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
