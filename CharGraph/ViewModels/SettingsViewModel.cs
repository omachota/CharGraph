using  CharGraph.Infrastructure;

namespace CharGraph.ViewModels
{
	public class SettingsViewModel : BaseViewModel
	{
		private string _test;

		public SettingsViewModel()
		{
			_test = "Settings";
		}

		public string Test
		{
			get => _test;
			set => SetAndRaise(ref _test, value);
		}
	}
}
