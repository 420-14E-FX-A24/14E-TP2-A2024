using Automate.Models;
using Automate.Utils;
using Automate.Views;
using Microsoft.VisualBasic;
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
using static Automate.Models.Jour;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
      
        //Propriétés du ViewModel
      
        private Jour _jours;
        private string? _leMois;
        private string? _annee;

        //commandes utilisées par l'interface
        public ICommand ConsulterJourCalendrierPageCommand { get; }
        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;
        

        //dictionnaire des erreurs de validation
        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        //commandes utilisées par l'interface
        public bool HasErrors => _errors.Count > 0;
        public bool HasLeMoisErrors => _errors.ContainsKey(nameof(LeMois)) && _errors[nameof(LeMois)].Any();
        public bool HasAnneeErrors => _errors.ContainsKey(nameof(Annee)) && _errors[nameof(Annee)].Any();


        //référence à la vue
        private Window _window;
        //constructeur
        public AccueilViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            ConsulterJourCalendrierPageCommand = new RelayCommand(ConsulterJourCalendrierPage);
            _navigationService = new NavigationService();
            _window = openedWindow;

        }
       
        //propriétés


        public string? LeMois
        {
            get => _leMois;
            set
            {
                _leMois = value;
                OnPropertyChanged(nameof(LeMois));
                ValidateProperty(nameof(LeMois));
            }
        }

        public string? Annee
        {
            get => _annee;
            set
            {
                _annee = value;
                OnPropertyChanged(nameof(Annee));
                ValidateProperty(nameof(Annee));
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

        public void ConsulterJourCalendrierPage()
        {
            ValidateProperty(nameof(Annee));
            ValidateProperty(nameof(LeMois));
            int annee = int.Parse(Annee);
            int mois = int.Parse(LeMois);
            DateTime date = new DateTime(annee, mois, 1);
            var donneeCalendrierPage = _mongoService.ConsulterJourCalendrierPage(date);
            if (donneeCalendrierPage == null)
            {
                AddError("Calendrier", "Année ou mois invalide.");
                Trace.WriteLine("invalide");
            }
            else
            {
                _navigationService.NavigateTo<CalendrierPageWindow>();
                _navigationService.Close(_window);
                Trace.WriteLine($"Accéder à calendrier Annee: {Annee}, Mois: {LeMois}");
            }
        }

        private void ValidateProperty(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(Annee):
                    int iAnnee;
                    bool estIntAnnee = int.TryParse(Annee, out iAnnee);
                    if (!estIntAnnee || iAnnee < 2024 || iAnnee > 2124)
                    {
                        Annee = "2024";
                        AddError(nameof(Annee), "L'année doit être un entier. Il doit être compris entre 2024 et 2123, inclusivement.");
                    }  
                    else
                        RemoveError(nameof(Annee));
                    break;

                case nameof(LeMois):
                    int iLeMois;
                    bool estIntLeMois = int.TryParse(LeMois, out iLeMois);
                    if (!estIntLeMois || iLeMois < 1 || iLeMois > 12)
                    {
                        LeMois = "1";
                        AddError(nameof(LeMois), "Le mois doit être un entier. Il doit être compris entre 1 et 12, inclusivement.");
                    }     
                    else
                        RemoveError(nameof(LeMois));
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
            OnPropertyChanged(nameof(HasLeMoisErrors));
            OnPropertyChanged(nameof(HasAnneeErrors));
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
            OnPropertyChanged(nameof(HasLeMoisErrors));
            OnPropertyChanged(nameof(HasAnneeErrors));
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
