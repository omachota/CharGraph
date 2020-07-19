using System;
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

		public event EventHandler OnCurrentWindowTypeChanged;

		private ViewType _currentWindowType;
		
		public ViewType CurrentWindowType
		{
			get => _currentWindowType;
			set
			{
				OnCurrentWindowTypeChanged?.Invoke(this, EventArgs.Empty);
				_currentWindowType = value;
			}
		}

	}
}
