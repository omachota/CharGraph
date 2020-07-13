using System.Windows.Input;
using CharGraph.ViewModels;

namespace CharGraph.Infrastructure.SwitchView
{
	public class Navigator : AbstractNotifyPropertyChanged, INavigator
	{
		private BaseViewModel _currentViewModel;

		public Navigator()
		{
			UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(this);
		}

		public BaseViewModel CurrentViewModel
		{
			get => _currentViewModel;
			set => SetAndRaise(ref _currentViewModel, value);
		}
		public ICommand UpdateCurrentViewModelCommand { get; }
		public ViewType CurrentWindowType { get; set; }
	}
}
