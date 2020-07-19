using System.Collections.Generic;
using System.IO.Ports;
using CharGraph.Infrastructure;

namespace CharGraph.Models
{
    public class Arduino : AbstractNotifyPropertyChanged
    {
        private SerialPort SerialPort { get; }
        
        public Arduino(string name, SerialPort serialPort)
        {
            Name = name;
            SerialPort = serialPort;
            SerialPort.Open();
            SerialPort.DataReceived += SerialPortOnDataReceived;
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = SerialPort.ReadLine();
            DataList.Add(ArduData.Serialize(data));
        }

        public string Name { get; set; }

        public readonly List<ArduData> DataList = new List<ArduData>();
    }
}
