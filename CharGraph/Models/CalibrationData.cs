using CharGraph.Infrastructure;

namespace CharGraph.Models
{
	public class CalibrationData : AbstractNotifyPropertyChanged
	{
		private int _source1;
		private int _source2;
		private bool _isVisible1 = true;
		private bool _isVisible2 = true;

		public int Voltage { get; set; }

		public int Source1
		{
			get => _source1;
			set => SetAndRaise(ref _source1, value);
		}

		public int Source2
		{
			get => _source2;
			set => SetAndRaise(ref _source2, value);
		}

		public bool IsVisible1
		{
			get => _isVisible1;
			set => SetAndRaise(ref _isVisible1, value);
		}

		public bool IsVisible2
		{
			get => _isVisible2;
			set => SetAndRaise(ref _isVisible2, value);
		}
	}
}
