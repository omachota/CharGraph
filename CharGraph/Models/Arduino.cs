using System;
using System.Diagnostics;
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
			SerialPort.DataReceived += SerialPortOnDataReceived;
			SerialPort.BaudRate = 115200;
			SerialPort.ReadTimeout = 5000;
			SerialPort.Open();
		}

		public void ClosePort()
		{
			SerialPort.Close();
		}

		public void Flush()
		{
			SerialPort.DiscardInBuffer();
		}

		public void Write(string s)
		{
			if (SerialPort.IsOpen)
			{
				SerialPort.WriteLine(s);
			}
		}

		public delegate void ArduinoDetected(string portName);

		public ArduinoDetected ReadEvent;

		private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			string data = "";

			try
			{
				data = SerialPort.ReadLine();
			}
			catch (Exception exception)
			{
				Debug.WriteLine(exception);
			}

			ReadEvent?.Invoke(data);
		}

		public string Name { get; set; }
	}
}
