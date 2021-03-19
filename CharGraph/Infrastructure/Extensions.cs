using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CharGraph.Models;

namespace CharGraph.Infrastructure
{
    public static class Extensions
    {
        public static readonly string Directory =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CharGraph";

        public static readonly string FilePath = Directory + @"\settings.txt";

        public static void ExportCsv(List<ArduData> dataList, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))

            {
                foreach (var data in dataList.OrderBy(x => x.Title))
                {
                    sw.WriteLine(data.ToString());
                }
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
                            settings.Fuse1Index = int.Parse(splittedLine[1]);
                            break;
                        case "fuse2":
                            settings.Fuse2Index = int.Parse(splittedLine[1]);
                            break;
                        case "resolution":
                            settings.Resolution = int.Parse(splittedLine[1]);
                            break;
                        case "exp":
                            settings.Exp = double.Parse(splittedLine[1]);
                            break;
                        case "nullPoint":
                            settings.NullPoint = int.Parse(splittedLine[1]);
                            break;
                        case "lines":
                            settings.Lines = int.Parse(splittedLine[1]);
                            break;
                        case "mode":
                            settings.Mode = bool.Parse(splittedLine[1]);
                            break;
                    }
                }
            }

            return settings;
        }
    }
}