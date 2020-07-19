namespace CharGraph.Models
{
    public class ArduData
    {
        public static ArduData Serialize(string data)
        {
            string[] splittedData = data.Split('\t');
            return new ArduData();
        }
    }
}