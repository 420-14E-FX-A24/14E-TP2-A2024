using Automate.Utils;
using Automate.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Automate.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        //Propriétés du ViewModel
        private string? _username;
        private string? _password;
        private readonly MongoDBService _mongoService;
        private readonly NavigationService _navigationService;
        //référence à la vue
        private Window _window;

        //dictionnaire des erreurs de validation
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        //commandes utilisées par l'interface
        public ICommand AuthenticateCommand { get; }
        public bool HasErrors => _errors.Count > 0;
        public bool HasPasswordErrors => _errors.ContainsKey(nameof(Password)) && _errors[nameof(Password)].Any();

        //constructeur
        public LoginViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            AuthenticateCommand = new RelayCommand(Authenticate);

            _navigationService = new NavigationService();
            _window = openedWindow;

        }

        //propriétés
        public string? Username
        {
            get => _username;
            set
            {
                //quand la valeur du textbox est modifiée, on valide les données et on avertit la vue
                _username = value;
                OnPropertyChanged(nameof(Username));
                ValidateProperty(nameof(Username));
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                ValidateProperty(nameof(Password));
            }
        }

        public string ErrorMessages
        {
            get
            {
                var allErrors = new List<string>();
                foreach (var errorList in _errors.Values)
                {
                    allErrors.AddRange(errorList);
                }
                // Retirer les chaînes vides et nulles
                allErrors.RemoveAll(error => string.IsNullOrWhiteSpace(error));

                return string.Join("\n", allErrors); // Joint les erreurs par une nouvelle ligne
            }
        }

        //méthodes
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Authenticate()
        {
            ValidateProperty(nameof(Username));
            ValidateProperty(nameof(Password));

            if (!HasErrors)
            {
                var user = _mongoService.Authenticate(Username, Password);
                if (user == null)
                {
                    AddError("Username", "Nom d'utilisateur ou mot de passe invalide");
                    AddError("Password", "");
                    Trace.WriteLine("invalid");
                }
                else
                {
                    _navigationService.NavigateTo<AccueilWindow>();
                    _navigationService.Close(_window);
                    Trace.WriteLine("logged in");
                }

            }
        }

        private void ValidateProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Username):
                    if (string.IsNullOrEmpty(Username))
                    {
                        AddError(nameof(Username), "Le nom d'utilisateur ne peut pas être vide.");
                    }
                    else
                    {
                        RemoveError(nameof(Username));
                    }
                    break;

                case nameof(Password):
                    if (string.IsNullOrEmpty(Password))
                    {
                        AddError(nameof(Password), "Le mot de passe ne peut pas être vide.");
                    }
                    else
                    {
                        RemoveError(nameof(Password));
                    }
                    break;
            }
        }

        private void AddError(string propertyName, string errorMessage)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            if (!_errors[propertyName].Contains(errorMessage))
            {
                _errors[propertyName].Add(errorMessage);
               ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            // Notifier les changements des propriétés
            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
               ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName)); 
            }
            // Notifier les changements des propriétés
            OnPropertyChanged(nameof(ErrorMessages));
            OnPropertyChanged(nameof(HasPasswordErrors));
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
            {
                return Enumerable.Empty<string>();
            }

            return _errors[propertyName];
        }

    }
}
