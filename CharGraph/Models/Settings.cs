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
			arduino.Write($"Fuse1 {Fuse1}");
			await Task.Delay(25);
			arduino.Write($"Fuse2 {Fuse2}");
		}
	}
}
