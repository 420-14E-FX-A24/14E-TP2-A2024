using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Automate.Utils
{
    public class NavigationService
    {
        // Méthode pour ouvrir une nouvelle vue
        public void NavigateTo<T>() where T : Window, new()
        {
            var window = new T();
            window.Show();
        }

        // Méthode pour fermer la vue actuelle
        public void Close(Window window)
        {
            window.Close();
        }
    }

}
