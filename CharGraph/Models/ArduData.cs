namespace CharGraph.Models
{
    public class ArduData
    {

        public string Title { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public ArduData(string line, string title)
        {
            Title = title;
            line = line.Replace('.', ','); // TODO : handle this with CultureInfo
            var splitedLine = line.Split(';');
            X = double.Parse(splitedLine[0]);
            Y = double.Parse(splitedLine[1])*1000;
        }

        public override string ToString()
        {
            return $"{Title};{X};{Y}";
        }
    }
}
