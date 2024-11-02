using Amazon.Runtime.Internal.Util;
using Automate.Models;
using Automate.Utils;
using Automate.Views;
using MaterialDesignThemes.Wpf;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static Automate.Models.Jour;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using static System.Net.Mime.MediaTypeNames;

namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged
    {

        //Propriétés du ViewModel
        private int _selectedTache;
        private ObservableCollection<string> _tests;
        private ObservableCollection<Jour> _jourObservableSelection;
        private ObservableCollection<Jour> _jourObservableSelectionTempo;
        private List<Jour> _jourSelection;
        private int _selectionIndexJour;

        private DateTime _dateSelection;
        //commandes utilisées par l'interface
        //Propriétés du ViewModel


        private ObservableCollection<Tache> _taches;
        private ObservableCollection<string> _commentaires;
        public IEnumerable<Tache> EnumValues => Enum.GetValues(typeof(Tache)).Cast<Tache>();
        private Jour _jour;
        private string? _tache;
        private string _commentaire;
        private int _selectionIndexTache;
        private int _selectionTache;
        private int _selectionCommentaire;

        //commandes utilisées par l'interface
        public ICommand AjouterCommentaireCommand { get; }
        public ICommand RetirerCommentaireCommand { get; }
        public ICommand AjouterTacheCommand { get; }
        public ICommand RetirerTacheCommand { get; }
        public ICommand EnregistrerJourCommand { get; }
        public ICommand RetournerAccueilCommand { get; }
        public ICommand ConsulterJourCalendrierPageCommand { get; }
        public ICommand ModifierJourCommand { get; }

        public ICommand FermerDialogCommand { get; }
        public RelayCommand AfficherDialogCommand { get; }

        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;

        //référence à la vue
        private Window _window;
        //constructeur


        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;

        //constructeur
        public AccueilViewModel(Window openedWindow)
        {
            //instanciation de la BD
            _mongoService = new MongoDBService("AutomateDB");
            ConsulterJourCalendrierPageCommand = new RelayCommand(ConsulterJourCalendrierPage);
            ModifierJourCommand = new RelayCommand(ModifierJour);
            AjouterTacheCommand = new RelayCommand(AjouterTache);
            RetirerTacheCommand = new RelayCommand(RetirerTache);
            AjouterCommentaireCommand = new RelayCommand(AjouterCommentaire);
            RetirerCommentaireCommand = new RelayCommand(RetirerCommentaire);
            EnregistrerJourCommand = new RelayCommand(EnregistrerJour);
            RetournerAccueilCommand = new RelayCommand(RetournerAccueil);
            AfficherDialogCommand = new RelayCommand(AfficherDialog);
            FermerDialogCommand = new RelayCommand(FermerDialog);
            _navigationService = new NavigationService();
            _window = openedWindow;
            DateSelection = DateTime.Now;
            _commentaire = "";
            //Créer une légende à partir de toute la collection.
            TextBlock TextBlock = (TextBlock)_window.FindName("FeedbackAjouterCommentaire");
            if (TextBlock is not null)
                ObtenirJour();
            else
                ConsulterJourCalendrierPage();
        }

        public DateTime DateSelection
        {
            get => _dateSelection;
            set
            {
                _dateSelection = value;
                OnPropertyChanged(nameof(DateSelection));

            }
        }


        public ObservableCollection<string> Tests
        {
            get => _tests;
            set
            {
                _tests = value;
                OnPropertyChanged(nameof(Tests));

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

        public int SelectionIndexJour
        {
            get => _selectionIndexJour;
            set
            {
                _selectionIndexJour = value;
                OnPropertyChanged(nameof(SelectionIndexJour));
            }
        }

        public int SelectionIndexTache
        {
            get => _selectionIndexTache;
            set
            {
                _selectionIndexTache = value;
                OnPropertyChanged(nameof(SelectionIndexTache));
            }
        }


        public ObservableCollection<Jour> JourObservableSelection
        {
            get => _jourObservableSelection;
            set
            {
                _jourObservableSelection = value;
                OnPropertyChanged(nameof(JourObservableSelection));
            }
        }

        public ObservableCollection<Tache> Taches
        {
            get => _taches;
            set
            {
                _taches = value;
                OnPropertyChanged(nameof(Taches));
            }
        }


        public ObservableCollection<string> Commentaires
        {
            get => _commentaires;
            set
            {
                if (_commentaires is not null)
                    _commentaires.Clear();
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

        public string Commentaire
        {
            get => _commentaire;
            set
            {
                _commentaire = value;
                OnPropertyChanged(nameof(Commentaire));
            }
        }


        public int SelectionTache
        {
            get => _selectionTache;
            set
            {
                _selectionTache = value;
                OnPropertyChanged(nameof(SelectionTache));
            }
        }


        public int SelectionCommentaire
        {
            get => _selectionCommentaire;
            set
            {
                _selectionCommentaire = value;
                OnPropertyChanged(nameof(SelectionCommentaire));
            }
        }



        //méthodes
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void AjouterStyleErreur(bool erreur, bool type, string msg = "")
        {
            TextBlock TextBlock;
            Border Border;

            if (type)
            {
                ComboBox CbxTaches = (ComboBox)_window.FindName("CbxTaches");
                TextBlock = (TextBlock)_window.FindName("FeedbackAjouterTache");
                Border = (Border)_window.FindName("BorderTache");

                if (CbxTaches != null && TextBlock != null && Border != null)
                {
                    if (erreur)
                    {
                        if (CbxTaches.SelectedIndex == 0)
                        {
                            TextBlock.Text = msg;
                            AjouterStyleBorder(Border, true);
                            Debug.WriteLine(msg);
                        }
                    }
                    else
                    {
                        AjouterStyleBorder(Border, false);
                        TextBlock.Text = "";
                    }
                }
            }
            else
            {
                TextBlock = (TextBlock)_window.FindName("FeedbackAjouterCommentaire");
                Border = (Border)_window.FindName("BorderCommentaire");

                if (TextBlock != null && Border != null)
                {
                    if (erreur)
                    {
                        if (Commentaire == "")
                        {
                            TextBlock.Text = msg;
                            AjouterStyleBorder(Border, true);
                            Debug.WriteLine(msg);
                        }
                    }
                    else
                    {
                        AjouterStyleBorder(Border, false);
                        TextBlock.Text = "";
                    }
                }
            }
        }


        public void AjouterStyleBorder(Border Border, bool erreur)
        {
            if (erreur)
            {
                Border.BorderThickness = new Thickness(3);
                Border.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else
            {
                Border.BorderThickness = new Thickness(0);
                Border.BorderBrush = new SolidColorBrush(Colors.White);
            }
        }

        public void AjouterCommentaire()
        {
            if (!string.IsNullOrEmpty(Commentaire))
            {
                int index = SelectionCommentaire;
                JourObservableSelection[0].CommentaireTaches.Add(Commentaire);
                ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
                JourObservableSelection.Clear();
                JourObservableSelection = new ObservableCollection<Jour>(tempos);
                OnPropertyChanged(nameof(JourObservableSelection));
                SelectionCommentaire = index;
                OnPropertyChanged(nameof(SelectionCommentaire));
                AjouterStyleErreur(false, false, "");
            }
            else
                AjouterStyleErreur(true, true, "Le commentaire ne peut pas être vide.");

        }

        public void RetirerCommentaire()
        {
            int index = SelectionCommentaire;
            if (SelectionCommentaire >= 0)
            {
                _jourObservableSelection[0].CommentaireTaches.RemoveAt(index);
                ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
                JourObservableSelection.Clear();
                JourObservableSelection = new ObservableCollection<Jour>(tempos);
                OnPropertyChanged(nameof(JourObservableSelection));
                int nombre = JourObservableSelection[0].CommentaireTaches.Count;
                if (index == 0 && nombre > 0)
                    SelectionCommentaire = index;
                else
                    SelectionCommentaire = -1;
                OnPropertyChanged(nameof(SelectionCommentaire));
            } 
        }


        public void AjouterTache()
        {
            int index = SelectionTache;
            JourObservableSelection[0].Taches.Add((Jour.Tache)SelectionIndexTache);
            ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
            JourObservableSelection.Clear();
            JourObservableSelection = new ObservableCollection<Jour>(tempos);
            OnPropertyChanged(nameof(JourObservableSelection));
            SelectionTache = index;
            OnPropertyChanged(nameof(SelectionTache));
        }


        public void RetirerTache()
        {
            int index = SelectionTache;
            if (index >= 0)
            {
                _jourObservableSelection[0].Taches.RemoveAt(index);
                ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(JourObservableSelection);
                JourObservableSelection.Clear();
                JourObservableSelection = new ObservableCollection<Jour>(tempos);
                OnPropertyChanged(nameof(JourObservableSelection));
                int nombre = JourObservableSelection[0].Taches.Count;
                if (index == 0 && nombre > 0)
                    SelectionTache = index;
                else
                    SelectionTache = -1;
                OnPropertyChanged(nameof(SelectionTache));
            }
        }


        public void EnregistrerJour()
        {
            _mongoService.EnregistrerJour(JourObservableSelection[0]);
            AfficherDialog();
        }


        public void RetournerAccueil()
        {
            _navigationService.NavigateTo<AccueilWindow>();
            _navigationService.Close(_window);
            Trace.WriteLine("Naviguer vers accueil.");
        }


        public void ObtenirJour()
        {
            JourSelection = new List<Jour>();
            JourSelection.Add(_mongoService.ModifierJour(DateSelection));
            OnPropertyChanged(nameof(JourSelection));
            JourObservableSelection = new ObservableCollection<Jour>(JourSelection);
            OnPropertyChanged(nameof(JourObservableSelection));
            _taches = new ObservableCollection<Tache>(JourObservableSelection[0].Taches);
            OnPropertyChanged(nameof(Taches));
            Commentaires = new ObservableCollection<string>(JourObservableSelection[0].CommentaireTaches);
            OnPropertyChanged(nameof(Commentaires));
            Tache = "Semis";
            SelectionTache = 0;
            SelectionIndexTache = 0;
            SelectionCommentaire = 0;
        }


        public void ConsulterJourCalendrierPage()
        {
            _jourSelection = _mongoService.ConsulterJourCalendrierPage(DateSelection);
            _jourObservableSelection = new ObservableCollection<Jour>(_jourSelection);
            _jourObservableSelectionTempo = new ObservableCollection<Jour>(_jourSelection);
            if (_jourObservableSelection.Count > 0)
            {
                SelectionIndexJour = 0;
            }
            OnPropertyChanged(nameof(JourObservableSelection));
        }


        public void ModifierJour()
        {
                _navigationService.NavigateTo<ModifierJourWindow>(_window.DataContext);
                _navigationService.Close(_window);
                Trace.WriteLine("Naviguer vers modifier un jour."); 
        }


        private async void AfficherDialog()
        {
            var dialogContent = new ConfirmationDialog(); // Replace with your actual dialog content
            await DialogHost.Show(dialogContent, "ModifierJourDialog");
        }

        private void FermerDialog()
        {
            DialogHost.Close("ModifierJourDialog", null);
        }

   
    }
}
