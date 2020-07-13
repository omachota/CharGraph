using System.Windows;
using System.Windows.Input;
using CharGraph.Infrastructure;
using CharGraph.Infrastructure.SwitchView;

namespace CharGraph.ViewModels
{
	public class MainViewModel : BaseViewModel
	{
		private Visibility _isSettingsButtonVisible = Visibility.Collapsed;
		private Visibility _isMainButtonVisible = Visibility.Visible;
		public INavigator Navigator { get; }

		public ICommand SwitchViewCommand { get; }

		public MainViewModel()
		{
			Navigator = new Navigator();
			Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Settings);
			SwitchViewCommand = new Command(() =>
			{
				if (Navigator.CurrentWindowType == ViewType.Main)
				{
					IsSettingsButtonVisible = Visibility.Collapsed;
					IsMainButtonVisible = Visibility.Visible;
					Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Settings);
				}
				else
				{
					IsSettingsButtonVisible = Visibility.Visible;
					IsMainButtonVisible = Visibility.Collapsed;
					Navigator.UpdateCurrentViewModelCommand.Execute(ViewType.Main);
				}
			});
		}

		public Visibility IsSettingsButtonVisible
		{
			get => _isSettingsButtonVisible;
			set => SetAndRaise(ref _isSettingsButtonVisible, value);
		}

		public Visibility IsMainButtonVisible
		{
			get => _isMainButtonVisible;
			set => SetAndRaise(ref _isMainButtonVisible, value);
		}
	}
}
