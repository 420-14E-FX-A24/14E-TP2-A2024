using Automate.Interfaces;
using System;
using System.Diagnostics;
using Automate.Models;
using Automate.ViewModels;

namespace Automate.Utils
{

    public class WindowServiceWrapper: IWindowService 
    {
        private AccueilViewModel _viewModel { get; set;}
        private static IWindowService? _sharedSingleton;

        //Prévenir la création d'un autre singleton
        private WindowServiceWrapper(AccueilViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public static IWindowService GetInstance(AccueilViewModel viewModel)
        {
            if(_sharedSingleton == null)
            {
                _sharedSingleton = new WindowServiceWrapper(viewModel);
            }
            // Access properties through the instance
            var instance = (WindowServiceWrapper)_sharedSingleton;
            var properties = instance._viewModel.GetType().GetProperties();
            Debug.WriteLine(_sharedSingleton.GetType().FullName); // Debug statement
            return _sharedSingleton;
        }

        public DateTime DateSelection
        {
            get => (DateTime)_viewModel.GetType().GetProperty("DateSelection").GetValue(_viewModel);
            set => _viewModel.GetType().GetProperty("DateSelection").SetValue(_viewModel, value);
        }


		public Role? Role
		{
			get => _viewModel.GetType().GetProperty("Role")?.GetValue(_viewModel) as Role?;
			set
			{
				if (value is not null)
				{
					_viewModel.GetType().GetProperty("Role")?.SetValue(_viewModel, value);
				}
			}
		}


		public void Close()
        {
            _viewModel.Window.Close();
        }

    }
}
