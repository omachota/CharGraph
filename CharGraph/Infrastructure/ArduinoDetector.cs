using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CharGraph.Models;

namespace CharGraph.Infrastructure
{
	public static class ArduinoDetector
	{
		private static bool _isArduinoDetected;
		private static bool _arduinoMessageShowed;
		private static Arduino _arduino;

		private static readonly int _baudRate = 2_000_000; // changable
		private const string VidPattern = @"VID_([0-9A-F]{4})";
		private const string PidPattern = @"PID_([0-9A-F]{4})";

		// Experimental
		public static async Task Checker()
		{
			List<string> ports;
			while (true)
			{
				ports = GetArduinoPorts();
				if (_isArduinoDetected && ports.Count == 0)
				{
					ArduinoDisconnectedEvent?.Invoke();
					_arduino = null;
					_isArduinoDetected = false;
					_arduinoMessageShowed = false;
				}

				if (_arduinoMessageShowed == false && ports.Count > 0)
				{
					ArduinoDetectedEvent?.Invoke(ports[0]);
					_arduinoMessageShowed = true;
				}

				await Task.Delay(2000).ConfigureAwait(false);
			}
		}

		public delegate void ArduinoDisconnected();

		public static ArduinoDisconnected ArduinoDisconnectedEvent;

		public delegate void ArduinoDetected(string portName);

		public static ArduinoDetected ArduinoDetectedEvent;

		// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-pnpentity
		// https://stackoverflow.com/questions/45165299/wmi-get-list-of-all-serial-com-ports-including-virtual-ports
		public static List<string> GetArduinoPorts()
		{
			var ports = new List<string>();
			var searcher = new ManagementObjectSearcher("root\\cimv2",
				"SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

			foreach (var queryObj in searcher.Get())
			{
				string hardwareId = ((string[]) queryObj["HardwareID"]).Aggregate("", (current, item) => current + item);
				Match m = Regex.Match(hardwareId, VidPattern, RegexOptions.IgnoreCase);
				Match m2 = Regex.Match(hardwareId, PidPattern, RegexOptions.IgnoreCase);
				if (m.Success && m2.Success)
				{
					string portName = queryObj["Caption"].ToString();
					ports.Add(portName.Substring(portName.Length - 6, 4));
					_isArduinoDetected = true;
				}
			}

			return ports;
		}

		public static Arduino GetArduino()
		{
			var searcher = new ManagementObjectSearcher("root\\cimv2",
				"SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

			foreach (var queryObj in searcher.Get())
			{
				string hardwareId = ((string[]) queryObj["HardwareID"]).Aggregate("", (current, item) => current + item);
				Match m = Regex.Match(hardwareId, VidPattern, RegexOptions.IgnoreCase);
				Match m2 = Regex.Match(hardwareId, PidPattern, RegexOptions.IgnoreCase);
				if (m.Success && m2.Success)
				{
					string portName = queryObj["Caption"].ToString();
					_isArduinoDetected = true;
					_arduino = new Arduino(portName, new SerialPort(portName.Substring(portName.Length - 6, 4), _baudRate));
					return _arduino;
				}
			}

			return null;
		}
	}
}
