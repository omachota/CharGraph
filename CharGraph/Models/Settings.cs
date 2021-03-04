using System.Collections.Generic;
using System.Threading.Tasks;

namespace CharGraph.Models
{
	public class Settings
	{
		public int Min1 { get; set; }
		public int Min2 { get; set; }
		public int Max1 { get; set; } = 24;
		public int Max2 { get; set; } = 24;
		public int Fuse1 { get; set; } = 100;
		public int Fuse2 { get; set; } = 20;
		private List<int> Fuses { get; } = new List<int>() {100, 250, 500, 1000, 1500};
		private List<int> Fuses2 { get; } = new List<int>() {20, 50, 100, 200, 300};

		public async Task WriteToArduino(Arduino arduino)
		{
			arduino.Write($"Min1 {-Min1}");
			await Task.Delay(50);
			arduino.Write($"Min2 {-Min2}");
			await Task.Delay(50);
			arduino.Write($"Max1 {Max1}");
			await Task.Delay(50);
			arduino.Write($"Max2 {Max2}");
			await Task.Delay(50);
			arduino.Write($"Fuse1 {Fuses[Fuse1]}");
			await Task.Delay(50);
			arduino.Write($"Fuse2 {Fuses2[Fuse2]}");
		}
	}
}
