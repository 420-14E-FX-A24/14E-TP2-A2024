using Automate.ViewModels;
using System.Windows;

namespace Automate.Utils
{
    public class NavigationService
    {
        public void NavigateTo<T>(object dataContext = null, bool isAdmin = false) where T : Window, new()
        {
            var window = new T();
            if (dataContext != null)
                window.DataContext = dataContext;
            window.Show();
           
            if (window.DataContext is HomeViewModel viewModel)
                viewModel.IsAdmin = isAdmin;
        }

        public void Close(Window window)
        {
            window.Close();
        }
    }

}
