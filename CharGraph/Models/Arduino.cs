using System.IO.Ports;
using CharGraph.Infrastructure;

namespace CharGraph.Models
{
    public class Arduino : AbstractNotifyPropertyChanged
    {
        public Arduino(string name, SerialPort serialPort)
        {
            Name = name;
            SerialPort = serialPort;
        }

        public string Name { get; set; }

        public SerialPort SerialPort { get; set; }
    }
}
