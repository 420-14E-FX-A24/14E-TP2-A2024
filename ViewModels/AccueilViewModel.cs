using Automate.Models;
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
using System.Windows.Input;
using static Automate.Models.CalendrierPageModel;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
      
        //Propriétés du ViewModel
        private int _annee;
        private int _mois;
     

        //commandes utilisées par l'interface
        public ICommand ConsulterCalendrierPageCommand { get; }
        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;
        private CalendrierPageModel _calendrierPage;

        //dictionnaire des erreurs de validation
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        //commandes utilisées par l'interface
        public bool HasErrors => _errors.Count > 0;
        public bool HasAnneeErrors => _errors.ContainsKey(nameof(Annee)) && _errors[nameof(Annee)].Any();
        public bool HasMoisErrors => _errors.ContainsKey(nameof(Mois)) && _errors[nameof(Mois)].Any();
       

        //référence à la vue
        private Window _window;
        //constructeur
        public AccueilViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            ConsulterCalendrierPageCommand = new RelayCommand(ConsulterCalendrierPage);
            _navigationService = new NavigationService();
            _window = openedWindow;

        }
       
        //propriétés

        public int Annee
        {
            get => _annee;
            set
            {
                _annee = value;
                OnPropertyChanged(nameof(Annee));
                ValidateProperty(nameof(Annee));
            }
        }


        public int Mois
        {
            get => _mois;
            set
            {
                _mois = value;
                OnPropertyChanged(nameof(Mois));
                ValidateProperty(nameof(Mois));
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

        public void ConsulterCalendrierPage()
        {
            ValidateProperty(nameof(Annee));
            ValidateProperty(nameof(Mois));

            var donneeCalendrierPage = _mongoService.ConsulterCalendrierPage(Annee, Mois);
            if (donneeCalendrierPage == null)
            {
                AddError("Calendrier", "Année ou mois invalide.");
                Trace.WriteLine("invalide");
            }
            else
            {
                _navigationService.NavigateTo<CalendrierPageWindow>();
                _navigationService.Close(_window);
                Trace.WriteLine($"Accéder à calendrier Annee: {Annee}, Mois: {Mois}");
            }
        }

        private void ValidateProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Annee):
                    if (Annee < 2024 || Annee > 2025)
                        AddError(nameof(Annee), "L'année doit être entre 2024 et 2025, inclusivement.");
                    else
                        RemoveError(nameof(Annee));
                    break;

                case nameof(Mois):
                    if (Mois < 1 || Mois > 12)
                        AddError(nameof(Mois), "Le mois doit être entre 1 et 12, inclusivement.");
                    else
                        RemoveError(nameof(Mois));
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
            OnPropertyChanged(nameof(HasAnneeErrors));
            OnPropertyChanged(nameof(HasMoisErrors));
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
            OnPropertyChanged(nameof(HasAnneeErrors));
            OnPropertyChanged(nameof(HasMoisErrors));
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
