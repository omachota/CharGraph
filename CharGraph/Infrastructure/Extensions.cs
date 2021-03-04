using System;
using System.IO;
using CharGraph.Models;

namespace CharGraph.Infrastructure
{
	public static class Extensions
	{
		public static readonly string Directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\JakubProchazka\CharGraph";

		public static readonly string FilePath = Directory + @"\settings.txt";

		public static void SaveSettings(int min1, int min2, int max1, int max2, int fuse1, int fuse2)
		{
			using (var sw = new StreamWriter(FilePath))
			{
				sw.WriteLine("min1=" + min1);
				sw.WriteLine("min2=" + min2);
				sw.WriteLine("max1=" + max1);
				sw.WriteLine("max2=" + max2);
				sw.WriteLine("fuse1=" + fuse1);
				sw.WriteLine("fuse2=" + fuse2);
			}
		}

		public static Settings ReadSettings()
		{
			Settings settings = new Settings();
			using (StreamReader sr = new StreamReader(FilePath))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					var splittedLine = line.Split('=');

					switch (splittedLine[0])
					{
						case "min1":
							settings.Min1 = int.Parse(splittedLine[1]);
							break;
						case "min2":
							settings.Min2 = int.Parse(splittedLine[1]);
							break;
						case "max1":
							settings.Max1 = int.Parse(splittedLine[1]);
							break;
						case "max2":
							settings.Max2 = int.Parse(splittedLine[1]);
							break;
						case "fuse1":
							settings.Fuse1 = int.Parse(splittedLine[1]);
							break;
						case "fuse2":
							settings.Fuse2 = int.Parse(splittedLine[1]);
							break;
					}
				}
			}

			return settings;
		}
	}
}
