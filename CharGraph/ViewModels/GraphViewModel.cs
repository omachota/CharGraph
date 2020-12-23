using System;
using CharGraph.Infrastructure;

namespace CharGraph.ViewModels
{
	public class GraphViewModel : BaseViewModel
	{
		private string _test;

		public GraphViewModel()
		{
			if (ArduinoDetector.Arduino == null)
				throw new Exception("Nemáme arduino");
			else
			{
				Test = ArduinoDetector.Arduino.Name;

			}
			// TODO : fetch data to graph
		}

		public string Test
		{
			get => _test;
			set => SetAndRaise(ref _test, value);
		}


	}
}
