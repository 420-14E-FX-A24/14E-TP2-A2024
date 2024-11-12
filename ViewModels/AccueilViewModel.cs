using Automate.Utils;
using Automate.Models;
using Automate.Views;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Automate.Models.Jour;
using Automate.Interfaces;
using System.Data;
using System.Threading.Tasks;


namespace Automate.ViewModels
{
    public class AccueilViewModel : INotifyPropertyChanged
    {
        //Propriétés du ViewModel
        private int _arrosages;
        private int _semis;
        private int _selectedTache;
        private ObservableCollection<string> _tests;
        private ObservableCollection<Jour> _jourObservableSelection;
        private ObservableCollection<Jour> _jourObservableSelectionTempo;
        private List<Jour> _jourSelection;
        private int _selectionIndexJour;
        private DateTime _dateSelection;
        private string _role;
        private ObservableCollection<Tache> _taches;
        private ObservableCollection<string> _commentaires;
        private Jour _jour;
        private string? _tache;
        private string _commentaire;
        private int _selectionIndexTache;
        private int _selectionTache;
        private int _selectionCommentaire;
        //comboBox
        public IEnumerable<Tache> EnumValues => Enum.GetValues(typeof(Tache)).Cast<Tache>();
        //commandes utilisées par l'interface
        public ICommand AjouterCommentaireCommand { get; }
        public ICommand RetirerCommentaireCommand { get; }
        public ICommand AjouterTacheCommand { get; }
        public ICommand RetirerTacheCommand { get; }
        public ICommand RetournerAccueilCommand { get; }
        public ICommand ConsulterJourCommand { get; }
        public ICommand ModifierJourCommand { get; }
        public ICommand ModifierCommentaireCommand { get; }
        public ICommand ModifierTacheCommand { get; }
        public ICommand FermerDialogCommand { get; }
        public RelayCommand AfficherDialogCommand { get; }
        public RelayCommand LogoutCommand { get; }
        
        //Gestionnaires d'événements 
        public event PropertyChangedEventHandler? PropertyChanged;
        //référence à la vue
        private Window _window;
        //Services
        private readonly NavigationService _navigationService;
        private readonly MongoDBService _mongoService;
        private static IWindowService _windowService;
       
        public AccueilViewModel(Window openedWindow)
        {
            try
            {
                if (openedWindow == null)
                {
                    throw new ArgumentNullException(nameof(openedWindow), "Retour à la page de connexion. Aurevoir.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Logout. Retour à LoginWindow.");
            }
            if(_mongoService == null)
                _mongoService = new MongoDBService("AutomateDB");
            try
            {
                if (_windowService == null)
                    _windowService = WindowServiceWrapper.GetInstance(this);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            if (_windowService.DateSelection != DateTime.MinValue)
                DateSelection = _windowService.DateSelection;
            else
                DateSelection = DateTime.Now;

            ConsulterJourCommand = new RelayCommand(ConsulterJour);
            ModifierJourCommand = new RelayCommand(ModifierJour);
            AjouterTacheCommand = new RelayCommand(AjouterTache);
            ModifierTacheCommand = new RelayCommand(ModifierTache);
            RetirerTacheCommand = new RelayCommand(RetirerTache);
            AjouterCommentaireCommand = new RelayCommand(AjouterCommentaire);
            ModifierCommentaireCommand = new RelayCommand(ModifierCommentaire);
            RetirerCommentaireCommand = new RelayCommand(RetirerCommentaire);
            RetournerAccueilCommand = new RelayCommand(RetournerAccueil);
            AfficherDialogCommand = new RelayCommand(AfficherDialog);
            FermerDialogCommand = new RelayCommand(FermerDialog);
            LogoutCommand = new RelayCommand(Logout);
            _navigationService = new NavigationService();
            Window = openedWindow;
            _commentaire = "";
            if(openedWindow != null)
            {
                if (openedWindow.Title == "AccueilWindow")
                {
                    ConsulterJourCalendrierPage();

                }
                else if (openedWindow.Title == "ModifierJourWindow")
                {
                    if (Role == "Admin")
                        ObtenirJour();
                }
            }
        }


        public int Arrosages
        {
            get => _arrosages;
            set
            {
                if (_arrosages != value)
                {
                    _arrosages = value;
                }
            }
        }

        public int Semis
        {
            get => _semis;
            set
            {
                if (_semis != value)
                {
                    _semis = value;
                }
            }
        }


        public Window Window
        {
            get => _window;
            set
            {
                if (_window != value)
                {
                    _window = value;
                }
            }
        }


        public string Role
        {
            get => _role;
            set
            {
                if (_role != value && value is string)
                {
                    _role = value;
                    if(_windowService.Role != null)
                        _windowService.Role = value;
                   
                   
                }
                OnPropertyChanged(nameof(Role));
            }
        }


        public DateTime DateSelection
        {
            get => _dateSelection;
            set
            {
                if (_dateSelection != value)
                {
                    
                    if (_dateSelection == DateTime.MinValue && value != DateTime.MinValue)
                        _dateSelection = DateTime.Now;
                    int mois = ObtenirMois(_dateSelection);
                    _dateSelection = value;
                    if (value.Month != mois)
                        ConsulterJourCalendrierPage();
                    OnPropertyChanged(nameof(DateSelection));
                }
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
                ComboBox CbxTaches = (ComboBox)Window.FindName("CbxTaches");
                TextBlock = (TextBlock)Window.FindName("FeedbackAjouterTache");
                Border = (Border)Window.FindName("BorderTache");

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
                TextBlock = (TextBlock)Window.FindName("FeedbackAjouterCommentaire");
                Border = (Border)Window.FindName("BorderCommentaire");

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
                if(JourObservableSelection.Count > 0)
                {
                    JourObservableSelection[0].CommentaireTaches.Add(Commentaire);
                    JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                }
                else
                {
                    Jour nouveauJour = new Jour();
                    JourObservableSelection = new ObservableCollection<Jour>() { nouveauJour };
                    JourObservableSelection[0].CommentaireTaches.Add(Commentaire);
                    JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                }
                SelectionCommentaire = index;
                OnPropertyChanged(nameof(SelectionCommentaire));
                AjouterStyleErreur(false, false, "");
                EnregistrerAjoutCommentaire(Commentaire, JourObservableSelection[0]);
                Commentaire = string.Empty;
            }
            else
                AjouterStyleErreur(false, true, "Le commentaire ne peut pas être vide.");
        }

        public void ModifierCommentaire()
        {
            int index = SelectionCommentaire;
            if (index >= 0)
            {
                string commentaire = Commentaire;
                JourObservableSelection[0].CommentaireTaches[index] = commentaire;
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                int nombre = JourObservableSelection[0].CommentaireTaches.Count;
                TraitementSelectionUI(index, nombre, SelectionCommentaire);
                EnregistrerModificationCommentaire(commentaire, index, JourObservableSelection[0]);
                Commentaire = string.Empty;
            }
        }

        public void RetirerCommentaire()
        {
            int index = SelectionCommentaire;
            if (SelectionCommentaire >= 0)
            {
                string commentaire = JourObservableSelection[0].CommentaireTaches[index];
                JourObservableSelection[0].CommentaireTaches.RemoveAt(index);
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                int nombre = JourObservableSelection[0].CommentaireTaches.Count;
                TraitementSelectionUI(index, nombre, SelectionCommentaire);
                EnregistrerRetraitCommentaire(commentaire, JourObservableSelection[0]);
            } 
        }


        public void AjouterTache()
        {
            int index = SelectionTache;
            Tache tache = (Jour.Tache)SelectionIndexTache;
            if(JourObservableSelection.Count > 0)
            {
                JourObservableSelection[0].Taches.Add(tache);
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
            }
            else
            {
                Jour nouveauJour = new Jour();
                JourObservableSelection = new ObservableCollection<Jour>() { nouveauJour };
                JourObservableSelection[0].Taches.Add(tache);
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
            }
            SelectionTache = index;
            OnPropertyChanged(nameof(SelectionTache));
            EnregistrerAjoutTache(tache, JourObservableSelection[0]);
        }

        public void ModifierTache()
        {
            int index = SelectionTache;
            if (index >= 0)
            {
                Tache tache = (Tache)Enum.Parse(typeof(Tache), ((Jour.Tache)SelectionIndexTache).ToString());
                JourObservableSelection[0].Taches[index] = (Jour.Tache)SelectionIndexTache;
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                int nombre = JourObservableSelection[0].Taches.Count;
                TraitementSelectionUI(index, nombre, SelectionTache);
                EnregistrerModificationTache(tache, index, JourObservableSelection[0]);
            }
        }

        public void RetirerTache()
        {
            int index = SelectionTache;
            if (index >= 0)
            {
                Tache tache = JourObservableSelection[0].Taches[index];
                JourObservableSelection[0].Taches.RemoveAt(index);
                JourObservableSelection = TraitementForcerMiseAJourUI(JourObservableSelection);
                int nombre = JourObservableSelection[0].Taches.Count;
                TraitementSelectionUI(index, nombre, SelectionTache);
                EnregistrerRetraitTache(tache, JourObservableSelection[0]);
            }
        }

        public void EnregistrerModificationTache(Tache tache, int index, Jour jour)
        {
            _mongoService.EnregistrerModificationTache(tache, index, jour);
            
        }

        public void EnregistrerModificationCommentaire(string commentaire, int index, Jour jour)
        {
            _mongoService.EnregistrerModificationCommentaire(commentaire, index, jour);
           
        }

        public void EnregistrerAjoutTache(Tache tache, Jour jour)
        {
            _mongoService.EnregistrerAjoutTache(tache, jour);
            
        }

        public void EnregistrerAjoutCommentaire(string commentaire, Jour jour)
        {
            _mongoService.EnregistrerAjoutCommentaire(commentaire, jour);
            
        }

        public void EnregistrerRetraitTache(Tache tache, Jour jour)
        {
            _mongoService.EnregistrerRetraitTache(tache, jour);
        
        }

        public void EnregistrerRetraitCommentaire(string commentaire, Jour jour)
        {
            _mongoService.EnregistrerRetraitCommentaire(commentaire, jour);
            
        }


        public void ObtenirJour()
        {
            if (DateSelection == DateTime.MinValue)
                DateSelection = DateTime.Now;
            if (_windowService is not null)
                DateSelection = _windowService.DateSelection;
            JourSelection = new List<Jour>();
            JourSelection.Add(_mongoService.ConsulterJour(DateSelection));
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
            JourSelection = _mongoService.ConsulterJourCalendrierPage(DateSelection);
            JourObservableSelection = new ObservableCollection<Jour>(JourSelection);
            if (JourObservableSelection.Count > 0)
            {
                SelectionIndexJour = 0;
                Jour jour = JourObservableSelection
                .Where(j => EstMemeDate(j.Date, DateSelection.Date))
                .FirstOrDefault();
                VerifierAfficherAlertes(jour);
            }
            OnPropertyChanged(nameof(JourObservableSelection)); 
        }

        public bool EstMemeDate(DateTime jourDate, DateTime dateSelection)
        {
            return jourDate.Date == dateSelection.Date;
        }


        public void ConsulterJour()
        {
            LeJour = _mongoService.ConsulterJour(DateSelection);
            if(LeJour != null)
                VerifierAfficherAlertes(LeJour);
            
        }

        public void VerifierAfficherAlertes(Jour jour)
        {
            if (jour is not null)
            {
                if (jour.Taches.Contains(Jour.Tache.Semis) || jour.Taches.Contains(Jour.Tache.Arrosage))
                {
                    Arrosages = 0;
                    Semis = 0;
                    foreach (Tache tache in jour.Taches)
                    {
                        if (tache == Jour.Tache.Semis)
                            Arrosages += 1;
                        else if (tache == Jour.Tache.Arrosage)
                            Semis += 1;
                    }
                    AfficherDialog();
                }
            }
        }


        public void RetournerAccueil()
        {
            if (_windowService is not null)
                DateSelection = _windowService.DateSelection; //window service pour les mock tests de DateSelection
            Trace.WriteLine($"_windowService is not null, {_windowService is not null}");
            _navigationService.NavigateTo<AccueilWindow>(Window.DataContext);
            Trace.WriteLine("Naviguer vers accueil.");
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.Name == "ModifierJourWindowLaVue")
                {
                    window.Close();
                    break;
                }
            }
        }

        public void ModifierJour()
        {
            if(_windowService is not null)
                _windowService.DateSelection = DateSelection; 
            _navigationService.NavigateTo<ModifierJourWindow>(Window.DataContext);
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.Name == "AccueilWindowLaVue")
                {
                    window.Close();
                    break;
                }
            } 
        }


        public void AfficherDialog()
        {
            var dialogContent = new ConfirmationDialog();
            try
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    //var window = Application.Current.Windows.OfType<AccueilWindow>().FirstOrDefault();
                    if (Window != null)
                    {
                        await Task.Delay(100);
                        DialogHost.Show(dialogContent, "Alertes");
                    }
                }); 
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"InvalidOperationException11: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
            }
        }

        public void FermerDialog()
        {
            DialogHost.Close("Alertes", null);
        }

        public ObservableCollection<Jour> TraitementForcerMiseAJourUI(ObservableCollection<Jour> collection)
        {
            ObservableCollection<Jour> tempos = new ObservableCollection<Jour>(collection);
            collection.Clear();
            collection = new ObservableCollection<Jour>(tempos);
            OnPropertyChanged(nameof(collection));
            return collection;
        }

        public void TraitementSelectionUI(int index, int nombre, int selecteur)
        {
            if (index == 0 && nombre > 0)
                selecteur = index;
            else
                selecteur = -1;
            OnPropertyChanged(nameof(selecteur));
        }

        public int ObtenirMois(DateTime date)
        {
            return date.Month;
        }

        public void Logout()
        {
            Role = null;
            Window.DataContext = null;
            _windowService = null;
            DateSelection = DateTime.MinValue;
            var resetAccueilViewModel = new AccueilViewModel(null);
            _navigationService.NavigateTo<LoginWindow>();
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.Name == "ModifierJourWindowLaVue" || window.Name == "AccueilWindowLaVue")
                {
                    window.Close();
                    break;
                }
            }

        }

       
    }
}
