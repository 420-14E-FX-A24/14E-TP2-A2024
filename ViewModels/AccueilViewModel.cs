using Automate.Models;
using Automate.Utils;
using Automate.Views;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Automate.Models.Jour;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged
    {

        //Propriétés du ViewModel
        private ObservableCollection<Jour> _jourObservableSelection;
        private List<Jour> _jourSelection;
        private Jour _jour;
        private DateTime _dateSelection = DateTime.Now;
        //commandes utilisées par l'interface
        public ICommand ConsulterJourCalendrierPageCommand { get; }
        public ICommand ModifierJourCommand { get; }
        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;


        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;



        //référence à la vue
        private Window _window;
        //constructeur
        public AccueilViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            //CalendrierPrecedentCommand = new RelayCommand(CalendrierPrecedent);
            //CalendrierSuivantCommand = new RelayCommand(CalendrierSuivant);
            ConsulterJourCalendrierPageCommand = new RelayCommand(ConsulterJourCalendrierPage);
            ModifierJourCommand = new RelayCommand(ModifierJour);
            _navigationService = new NavigationService();
            _window = openedWindow;
            //Créer une légende à partir de toute la collection.
            ConsulterJourCalendrierPage();
        }

        //propriétés
        public DateTime DateSelection
        {
            get => _dateSelection;
            set
            {
                _dateSelection = value;
                OnPropertyChanged(nameof(DateSelection));
            }
        }


        //propriétés
        public List<Jour> JourSelection
        {
            get => _jourSelection;
            set
            {
                _jourSelection = value;
                OnPropertyChanged(nameof(JourSelection));
            }
        }


        public ObservableCollection<Jour> JourObservableSelection
        {
            get { return _jourObservableSelection; }
            set
            {
                _jourObservableSelection = value;
                OnPropertyChanged(nameof(JourObservableSelection));
            }
        }


        public Jour Jour
        {
            get => _jour;
            set
            {
                _jour = value;
                OnPropertyChanged(nameof(Jour));
            }
        }

        //méthodes
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void ModifierJour()
        {
            _navigationService.NavigateTo<ModifierJourWindow>();
            _navigationService.Close(_window);
            Trace.WriteLine("Navigue pour modifier un jour.");
        }

        public void ConsulterJourCalendrierPage()
        {
            _jourSelection = _mongoService.ConsulterJourCalendrierPage(DateSelection);
            _jourObservableSelection = new ObservableCollection<Jour>(_jourSelection);
            OnPropertyChanged(nameof(JourObservableSelection));
        }

    }
}
