using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CharGraph.Models;

namespace CharGraph.Infrastructure
{
	public class ArduinoDetector
	{
		private bool _isArduinoDetected;
		private bool _arduinoMessageShowed;

		private readonly int _baudRate = 115200; // changable
		private const string VidPattern = @"VID_([0-9A-F]{4})";
		private const string PidPattern = @"PID_([0-9A-F]{4})";

		public bool InitializeArduino { get; set; }
		public Arduino Arduino { get; private set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="delay">Delay in ms</param>
		/// <returns></returns>
		public async Task ScanArduinoPorts(int delay)
		{
			List<string> ports;
			while (true)
			{
				ports = GetArduinoPorts();
				if (_isArduinoDetected && ports.Count == 0)
				{
					ArduinoDisconnectedEvent?.Invoke();
					Arduino = null;
					_isArduinoDetected = false;
					_arduinoMessageShowed = false;
				}

				if (_arduinoMessageShowed == false && ports.Count > 0)
				{
					Arduino ??= new Arduino(ports[0], new SerialPort(ports[0], _baudRate));
					if (InitializeArduino)
					{
						InitializeArduino = false;
					}

					ArduinoDetectedEvent?.Invoke(ports[0]);
					_arduinoMessageShowed = true;
				}

				await Task.Delay(delay).ConfigureAwait(false);
			}
			// ReSharper disable once FunctionNeverReturns
		}

		public delegate Task ArduinoDisconnected();

		public ArduinoDisconnected ArduinoDisconnectedEvent;

		public delegate Task ArduinoDetected(string portName);

		public ArduinoDetected ArduinoDetectedEvent;

		// https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/win32-pnpentity
		// https://stackoverflow.com/questions/45165299/wmi-get-list-of-all-serial-com-ports-including-virtual-ports
		public List<string> GetArduinoPorts()
		{
			var ports = new List<string>();
			var searcher = new ManagementObjectSearcher("root\\cimv2",
				"SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

			foreach (var queryObj in searcher.Get())
			{
				string hardwareId =
					((string[]) queryObj["HardwareID"]).Aggregate("", (current, item) => current + item);
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

		public void GetArduino()
		{
			var searcher = new ManagementObjectSearcher("root\\cimv2",
				"SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

			foreach (var queryObj in searcher.Get())
			{
				string hardwareId =
					((string[]) queryObj["HardwareID"]).Aggregate("", (current, item) => current + item);
				Match m = Regex.Match(hardwareId, VidPattern, RegexOptions.IgnoreCase);
				Match m2 = Regex.Match(hardwareId, PidPattern, RegexOptions.IgnoreCase);
				if (m.Success && m2.Success)
				{
					string portName = queryObj["Caption"].ToString();
					_isArduinoDetected = true;
					Arduino = new Arduino(portName,
						new SerialPort(portName.Substring(portName.Length - 6, 4), _baudRate));
					return;
				}
			}
		}
	}
}
