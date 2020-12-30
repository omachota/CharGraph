using CharGraph.Infrastructure;

namespace CharGraph.ViewModels
{
	public class GraphViewModel : BaseViewModel
	{
		private string _test;

		public GraphViewModel(ArduinoDetector arduinoDetector)
		{
			if (arduinoDetector.Arduino == null)
			{

				// throw new Exception("Nemáme arduino");
			}
			else
			{
				Test = arduinoDetector.Arduino.Name;

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
