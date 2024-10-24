using Automate.ViewModels;
using Automate.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Automate
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(this);
        }

        //pas le choix d'utiliser un événement, on ne peut pas BIND un password pour des raisons de sécurité
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox? passwordBox = sender as PasswordBox;
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = passwordBox.Password; // Met à jour la propriété Password dans le ViewModel
            }
        }
    }
}
