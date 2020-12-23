using System;

namespace CharGraph.Models
{
    public class ArduData
    {
        public string Title { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public static ArduData Serialize(string data)
        {
            string[] splittedData = data.Split('\t');
            return new ArduData("", "");
        }

        public ArduData(string line, string title)
        {
            Title = title;
            line = line.Replace('.', ','); // TODO : handle this with CultureInfo
            var splitedLine = line.Split(';');
            X = Double.Parse(splitedLine[0]);
            Y = Double.Parse(splitedLine[1]);
        }
    }
}
