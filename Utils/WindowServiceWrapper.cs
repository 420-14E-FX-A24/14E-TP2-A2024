using Automate.Interfaces;
using Automate.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Automate.Utils
{
	public class WindowServiceWrapper : IWindowService
	{
		private static IWindowService? _sharedSingleton;
		private object _viewModel;
		private Window _window;

		private WindowServiceWrapper(object viewModel, Window window)
		{
			_viewModel = viewModel;
			_window = window;
		}

		public static IWindowService GetInstance(object viewModel, Window window)
		{
			if (_sharedSingleton is null)
			{
				_sharedSingleton = new WindowServiceWrapper(viewModel, window);
			}

			return _sharedSingleton;
		}

		public DateTime DateSelection
		{
			get => (_viewModel as IWindowService)?.DateSelection ?? DateTime.MinValue;
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.DateSelection = value;
				}
			}
		}

		public bool IsAdmin
		{
			get => (_viewModel as IWindowService)?.IsAdmin ?? false;
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.IsAdmin = value;
				}
			}
		}

		public ObservableCollection<Task> Tasks
		{
			get => (_viewModel as IWindowService)?.Tasks ?? new ObservableCollection<Task>();
			set
			{
				if (_viewModel is IWindowService service)
				{
					service.Tasks = value;
				}
			}
		}

		public void Close()
		{
			_window.Close();
		}
	}
}