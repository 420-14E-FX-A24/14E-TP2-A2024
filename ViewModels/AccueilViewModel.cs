using Amazon.Runtime.Internal.Util;
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
using System.Windows.Data;
using System.Windows.Input;
using static Automate.Models.Jour;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged
    {

        //Propriétés du ViewModel
        private int _selectedIndex;
        private ObservableCollection<Jour> _jourObservableSelection;
        private ObservableCollection<Jour> _jourObservableSelectionTempo;
        private List<Jour> _jourSelection;
        private ObservableCollection<Tache> _taches;
        private ObservableCollection<string> _commentaires;
        public IEnumerable<Tache> EnumValues => Enum.GetValues(typeof(Tache)).Cast<Tache>();
        private Jour _jour;
        private string? _tache;
        private string? _commentaire;

        private DateTime _dateSelection = DateTime.Now;
        //commandes utilisées par l'interface
        public ICommand ConsulterJourCalendrierPageCommand { get; }
        public ICommand AjouterCommentaireCommand { get; }
        public ICommand RetirerCommentaireCommand { get; }
        public ICommand AjouterTacheCommand { get; }
        public ICommand RetirerTacheCommand { get; }
        public ICommand ModifierJourCommand { get; }
        public ICommand EnregistrerJourCommand { get; }
        public ICommand RetournerAccueilCommand { get; }

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
            ConsulterJourCalendrierPageCommand = new RelayCommand(ConsulterJourCalendrierPage);
            ModifierJourCommand = new RelayCommand(ModifierJour);
            AjouterCommentaireCommand = new RelayCommand(AjouterCommentaire);
            RetirerCommentaireCommand = new RelayCommand(RetirerCommentaire);
            AjouterTacheCommand = new RelayCommand(AjouterTache);
            RetirerTacheCommand = new RelayCommand(RetirerTache);
            EnregistrerJourCommand = new RelayCommand(EnregistrerJour);
            RetournerAccueilCommand = new RelayCommand(RetournerAccueil);
            ModifierJourCommand = new RelayCommand(ModifierJour);
            _navigationService = new NavigationService();
            _window = openedWindow;
            //Créer une légende à partir de toute la collection.
            ConsulterJourCalendrierPage();
            Tache = "Semis";
            SelectedIndex = 0;
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

        public ObservableCollection<Tache> Taches
        {
            get { return _taches; }
            set
            {
                _taches = value;
                OnPropertyChanged(nameof(Taches));
            }
        }


        public ObservableCollection<string> Commentaires
        {
            get { return _commentaires; }
            set
            {
                _commentaires = value;
                OnPropertyChanged(nameof(Commentaires));
            }
        }

        public Jour LeJour
        {
            get => _jour;
            set
            {
                _jour = value;
                OnPropertyChanged(nameof(LeJour));
            }
        }


        public string? Tache
        {
            get => _tache;
            set
            {
                _tache = value;
                OnPropertyChanged(nameof(Tache));
            }
        }

        public string? Commentaire
        {
            get => _commentaire;
            set
            {
                _commentaire = value;
                OnPropertyChanged(nameof(Commentaire));
            }
        }

       
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }

        //méthodes
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void AjouterCommentaire()
        {
            
            if (!string.IsNullOrEmpty(Commentaire))
            {
                JourObservableSelection[0].CommentaireTaches.Add(Commentaire);
                ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
                JourObservableSelection.Clear();
                JourObservableSelection = new ObservableCollection<Jour>(tempos);
                Commentaire = string.Empty;
                OnPropertyChanged(nameof(Commentaire));
            }
            else
            {
                // string empty
                Debug.WriteLine("Invalid task type: " + Commentaire);
            }
        }

        public void RetirerCommentaire()
        {
            _jourObservableSelection[0].CommentaireTaches.Remove(Commentaire);
            OnPropertyChanged(nameof(JourObservableSelection));
        }


        public void AjouterTache()
        {
                if (Enum.TryParse(typeof(Tache), Tache, out var result))
                {
                    Tache tacheChoisie = (Tache)result;
                    JourObservableSelection[0].Taches.Add(tacheChoisie);
                    ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
                    JourObservableSelection.Clear();
                    JourObservableSelection = new ObservableCollection<Jour>(tempos);

                }
                else
                {
                    // Handle the case where the parsing fails
                    Debug.WriteLine("Invalid task type: " + Tache);
                }
               
            
        }

        public void RetirerTache()
        {
            Tache tacheChoisie = (Tache)Enum.Parse(typeof(Tache), Tache);
            _jourObservableSelection[0].Taches.Remove(tacheChoisie);
            OnPropertyChanged(nameof(JourObservableSelection));
        }

        public void EnregistrerJour()
        {

        }

        public void RetournerAccueil()
        {
            _navigationService.NavigateTo<AccueilWindow>();
            _navigationService.Close(_window);
            Trace.WriteLine("Naviguer vers accueil.");
        }

        public void ModifierJour()
        {
            
            _jourSelection = new List<Jour>();
            _jourSelection.Add(_mongoService.ModifierJour(DateSelection));
            
            _jourObservableSelection = new ObservableCollection<Jour>(_jourSelection);
            _taches = new ObservableCollection<Tache>(_jourObservableSelection[0].Taches);
            _commentaires = new ObservableCollection<string>(_jourObservableSelection[0].CommentaireTaches);

            OnPropertyChanged(nameof(JourObservableSelection));
            OnPropertyChanged(nameof(Taches));
            OnPropertyChanged(nameof(Commentaires));
            _navigationService.NavigateTo<ModifierJourWindow>();
                _navigationService.Close(_window);
                Trace.WriteLine("Naviguer vers modifier un jour.");
        }

        public void ConsulterJourCalendrierPage()
        {
            _jourSelection = _mongoService.ConsulterJourCalendrierPage(DateSelection);
            _jourObservableSelection = new ObservableCollection<Jour>(_jourSelection);
            _jourObservableSelectionTempo = new ObservableCollection<Jour>(_jourSelection);
            OnPropertyChanged(nameof(JourObservableSelection));
        }

    }
}
