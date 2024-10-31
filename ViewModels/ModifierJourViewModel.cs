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
using System.Windows.Input;
using System.Windows;
using Automate.Models;
using static Automate.Models.Jour;

namespace Automate.ViewModels
{
    public class ModifierJourViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        //Propriétés du ViewModel
        private Jour _jour;
        private List<Tache> _taches;

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
        public ICommand ConsulterJourCommand { get; }
        public bool HasErrors => _errors.Count > 0;
        public bool HasTachesErrors => _errors.ContainsKey(nameof(Taches)) && _errors[nameof(Taches)].Any();

        //constructeur
        public ModifierJourViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            ConsulterJourCommand = new RelayCommand(ModifierJour);
            _navigationService = new NavigationService();
            _window = openedWindow;

        }


        //propriétés
        public List<Tache> Taches
        {
            get => _taches;
            set
            {
                //quand la valeur du textbox est modifiée, on valide les données et on avertit la vue
                _taches = value;
                OnPropertyChanged(nameof(Taches));
                ValidateProperty(nameof(Taches));
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

        public void ModifierJour()
        {
            ValidateProperty(nameof(Taches));

            if (!HasErrors)
            {
                var jour = _mongoService.ModifierJour(Taches);
                _navigationService.NavigateTo<AccueilWindow>();
                _navigationService.Close(_window);
                Trace.WriteLine("Retour à l'accueil.");

            }
        }

        private void ValidateProperty(string? propertyName)
        {
            
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
            OnPropertyChanged(nameof(HasTachesErrors));
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
            OnPropertyChanged(nameof(HasTachesErrors));
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
