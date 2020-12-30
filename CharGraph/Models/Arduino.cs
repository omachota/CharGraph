using System;
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

		public void ClosePort()
		{
			SerialPort.Close();
		}

		private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				var data = SerialPort.ReadLine();
				DataList.Add(ArduData.Serialize(data));
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}

		public string Name { get; set; }

		public readonly List<ArduData> DataList = new List<ArduData>();
	}
}
