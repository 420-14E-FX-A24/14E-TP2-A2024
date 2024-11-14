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
		public int Arrosages { get; set; }
		public int Semis { get; set; }
		public Window Window { get; set; }
		private ObservableCollection<string> _tests;
		private ObservableCollection<Jour> _jourObservableSelection;
		private List<Jour> _jourSelection;
		private int _selectionIndexJour;
		private DateTime _dateSelection;
		private Role? _role;
		private Jour? _jour;
		private string? _tache;
		private string _commentaire;
		private int _selectionIndexTache;
		private int _selectionTache;
		private int _selectionCommentaire;
		public IEnumerable<Tache> EnumValues => Enum.GetValues(typeof(Tache)).Cast<Tache>();
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

		public event PropertyChangedEventHandler? PropertyChanged;
		private readonly NavigationService _navigationService;
		private readonly MongoDBService _mongoService;
		private static IWindowService? _windowService;

		public AccueilViewModel(Window openedWindow)
		{
			try
			{
				if (openedWindow is null)
				{
					throw new ArgumentNullException(nameof(openedWindow), "Retour à la page de connexion. Au revoir.");
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Logout. Retour à LoginWindow.");
			}

			if (_mongoService is null)
				_mongoService = new MongoDBService("AutomateDB");

			try
			{
				if (_windowService is null)
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
			_commentaire = string.Empty;

			if (openedWindow is not null)
			{
				if (openedWindow.Title == "AccueilWindow")
					ConsulterJourCalendrierPage();
			}
		}

		public Role? Role
		{
			get => _role;
			set
			{
				if (_role != value && value is Role)
				{
					_role = value;

					if (_windowService.Role is not null)
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

		public Jour? LeJour
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
			if (type)
			{
				gererErreur(erreur, "CbxTaches", "FeedbackAjouterTache", "BorderTache", msg);
			}
			else
			{
				gererErreur(erreur, "FeedbackAjouterCommentaire", "BorderCommentaire", msg);
			}
		}

		private void gererErreur(bool erreur, string cbxName, string textBlockName, string borderName, string msg = "")
		{
			ComboBox? CbxTaches = cbxName is not null ? (ComboBox)Window.FindName(cbxName) : null;
			TextBlock TextBlock = (TextBlock)Window.FindName(textBlockName);
			Border Border = (Border)Window.FindName(borderName);

			if (TextBlock is not null && Border is not null && (CbxTaches == null || CbxTaches.SelectedIndex == 0))
			{
				if (erreur)
				{
					TextBlock.Text = msg;
					AjouterStyleBorder(Border, true);
					Debug.WriteLine(msg);
				}
				else
				{
					AjouterStyleBorder(Border, false);
					TextBlock.Text = string.Empty;
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
			if (string.IsNullOrEmpty(Commentaire))
			{
				AjouterStyleErreur(false, true, "Le commentaire ne peut pas être vide.");
				return;
			}

			LeJour!.CommentaireTaches.Add(Commentaire);

			OnPropertyChanged(nameof(LeJour));

			AjouterStyleErreur(false, false, string.Empty);
			EnregistrerAjoutCommentaire(Commentaire, JourObservableSelection[0]);
			Commentaire = string.Empty;
		}

		public void ModifierCommentaire()
		{
			if (SelectionCommentaire < 0) return;

			Jour jourSelection = JourObservableSelection[0];

			jourSelection.CommentaireTaches[SelectionCommentaire] = Commentaire;

			OnPropertyChanged(nameof(JourObservableSelection));

			TraitementSelectionUI(SelectionCommentaire, jourSelection.CommentaireTaches.Count, SelectionCommentaire);
			EnregistrerModificationCommentaire(Commentaire, SelectionCommentaire, jourSelection);

			Commentaire = string.Empty;
		}

		public void RetirerCommentaire()
		{
			if (SelectionCommentaire < 0) return;

			Jour jourSelection = JourObservableSelection[0];
			string commentaire = jourSelection.CommentaireTaches[SelectionCommentaire];

			jourSelection.CommentaireTaches.RemoveAt(SelectionCommentaire);

			OnPropertyChanged(nameof(JourObservableSelection));

			TraitementSelectionUI(SelectionCommentaire, jourSelection.CommentaireTaches.Count, SelectionCommentaire);
			EnregistrerRetraitCommentaire(commentaire, jourSelection);
		}

		public void AjouterTache()
		{
			Tache tache = (Tache)SelectionIndexTache;

			LeJour!.Taches.Add(tache);

			EnregistrerAjoutTache(tache, LeJour);

			OnPropertyChanged(nameof(SelectionTache));
		}

		public void ModifierTache()
		{
			if (SelectionTache < 0) return;

			int index = SelectionTache;
			var tache = (Tache)Enum.Parse(typeof(Tache), SelectionIndexTache.ToString());
			LeJour!.Taches[index] = (Tache)SelectionIndexTache;

			EnregistrerModificationTache(tache, index, LeJour);
			ConsulterJourCalendrierPage();
		}

		public void RetirerTache()
		{
			if (SelectionTache < 0) return;

			Tache tache = LeJour!.Taches[SelectionTache];
			LeJour.Taches.RemoveAt(SelectionTache);

			EnregistrerRetraitTache(tache, LeJour);
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
			{
				DateSelection = _windowService?.DateSelection ?? DateTime.Now;
			}

			LeJour = _mongoService.ConsulterJour(DateSelection);

			if (LeJour is null)
				LeJour = new Jour(DateSelection);

			Tache = "Semis";
			SelectionTache = 0;
			SelectionIndexTache = 0;
			SelectionCommentaire = 0;

			OnPropertyChanged(nameof(JourSelection));
			OnPropertyChanged(nameof(JourObservableSelection));
		}


		public void ConsulterJourCalendrierPage()
		{
			JourObservableSelection = new ObservableCollection<Jour>(_mongoService.ConsulterJourCalendrierPage(DateSelection));

			if (JourObservableSelection.Count > 0)
			{
				SelectionIndexJour = 0;
				Jour? jour = JourObservableSelection.FirstOrDefault(j => EstMemeDate(j.Date, DateSelection.Date));
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
			if (LeJour is not null)
				VerifierAfficherAlertes(LeJour);

		}

		public void VerifierAfficherAlertes(Jour? jour)
		{
			if (jour?.Taches.Any(t => t == Jour.Tache.Semis || t == Jour.Tache.Arrosage) == true)
			{
				Arrosages = jour.Taches.Count(t => t == Jour.Tache.Semis);
				Semis = jour.Taches.Count(t => t == Jour.Tache.Arrosage);

				AfficherDialog();
			}
		}


		public void RetournerAccueil()
		{
			if (_windowService is not null)
				DateSelection = _windowService.DateSelection; //window service pour les mock tests de DateSelection

			Trace.WriteLine($"_windowService is not null, {_windowService is not null}");

			_navigationService.NavigateTo<AccueilWindow>(Window.DataContext, Role.Value);
			Trace.WriteLine("Naviguer vers accueil.");

			foreach (Window window in Application.Current.Windows)
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
			if (_windowService is not null)
				_windowService.DateSelection = DateSelection;

			ObtenirJour();

			_navigationService.NavigateTo<ModifierJourWindow>(Window.DataContext);
			foreach (Window window in Application.Current.Windows)
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
					if (Window is not null)
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

			_navigationService.NavigateTo<LoginWindow>();

			foreach (Window window in Application.Current.Windows)
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
