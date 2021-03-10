using System.IO;
using System.Threading.Tasks;
using CharGraph.Infrastructure;

namespace CharGraph.Models
{
	public class Settings : AbstractNotifyPropertyChanged
	{
		private int _min1, _min2, _fuse1Index, _fuse2Index, _resolution, _nullPoint;
		private int _max1 = 24, _max2 = 24, _lines = 1;
		private double _exp;
		private bool _mode = true;

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

		public int Resolution
		{
			get => _resolution;
			set => SetAndRaise(ref _resolution, value);
		}

		public double Exp
		{
			get => _exp;
			set => SetAndRaise(ref _exp, value);
		}

		public int NullPoint
		{
			get => _nullPoint;
			set => SetAndRaise(ref _nullPoint, value);
		}

		public int Lines
		{
			get => _lines;
			set => SetAndRaise(ref _lines, value);
		}

		public bool Mode
		{
			get => _mode;
			set => SetAndRaise(ref _mode, value);
		}

		public async Task WriteToArduino(Arduino arduino)
		{
			arduino.Write($"Min1 {-Min1}");
			await Task.Delay(25);
			arduino.Write($"Min2 {-Min2}");
			await Task.Delay(25);
			arduino.Write($"Max1 {Max1}");
			await Task.Delay(25);
			arduino.Write($"Max2 {Max2}");
			await Task.Delay(25);
			arduino.Write($"Fuse1 {Fuse1Index}");
			await Task.Delay(25);
			arduino.Write($"Fuse2 {Fuse2Index}");
		}

		public void Save()
		{
			using (var sw = new StreamWriter(Extensions.FilePath))
			{
				sw.WriteLine("min1=" + Min1);
				sw.WriteLine("min2=" + Min2);
				sw.WriteLine("max1=" + Max1);
				sw.WriteLine("max2=" + Max2);
				sw.WriteLine("fuse1=" + Fuse1Index);
				sw.WriteLine("fuse2=" + Fuse2Index);
				sw.WriteLine("resolution=" + Resolution);
				sw.WriteLine("exp=" + Exp);
				sw.WriteLine("nullPoint=" + NullPoint);
				sw.WriteLine("lines=" + Lines);
				sw.WriteLine("mode=" + Mode);
			}
		}

		public static void SaveDefault()
		{
			using (var sw = new StreamWriter(Extensions.FilePath))
			{
				sw.WriteLine("min1=" + 0);
				sw.WriteLine("min2=" + 0);
				sw.WriteLine("max1=" + 20);
				sw.WriteLine("max2=" + 20);
				sw.WriteLine("fuse1=" + 0);
				sw.WriteLine("fuse2=" + 0);
				sw.WriteLine("resolution=" + 0);
				sw.WriteLine("exp=" + 0);
				sw.WriteLine("nullPoint=" + 0);
				sw.WriteLine("lines=" + 1);
				sw.WriteLine("mode=" + true);
			}
		}
	}
}
