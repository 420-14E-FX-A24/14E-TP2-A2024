using Automate.Interfaces;
using Automate.ViewModels;
using Automate.Models;
using System.Windows;

namespace Automate.Utils
{
    public class NavigationService
    {
        public void NavigateTo<T>(object dataContext = null, Role? role = null) where T : Window, new()
        {
            var window = new T();
            if (dataContext != null)
                window.DataContext = dataContext;
            window.Show();
           
            if (window.DataContext is AccueilViewModel viewModel)
                viewModel.Role = role;
        }

        public void Close(Window window)
        {
            window.Close();
        }
    }

}
