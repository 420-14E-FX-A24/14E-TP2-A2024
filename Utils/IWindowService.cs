using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Automate.ViewModels.AccueilViewModel;
using System.Windows;

namespace Automate.Utils
{
    public class WindowServiceWrapper : IWindowService
    {
        private Window _window;

        public WindowServiceWrapper(Window window)
        {
            _window = window;
        }

        public DateTime DateSelection
        {
            get => (DateTime)_window.GetType().GetProperty("DateSelection").GetValue(_window);
            set => _window.GetType().GetProperty("DateSelection").SetValue(_window, value);
        }

        public void Close()
        {
            _window.Close();
        }
    }
}
