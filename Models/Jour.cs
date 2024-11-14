using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using static Automate.Models.Jour;
using System.Windows.Media;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace Automate.Models
{

    public class Jour
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("TimeCreated")]
        public DateTime TimeCreated { get; set; }

        [BsonElement("TimeUpdated")]
        [BsonIgnoreIfNull]
        public DateTime TimeUpdated { get; set; }

        [BsonElement("Taches")]
        [BsonIgnoreIfNull]
		public ObservableCollection<Tache> Taches { get; set; }

        [BsonElement("CommentaireTaches")]
        [BsonIgnoreIfNull]
		public ObservableCollection<string> CommentaireTaches { get; set; }

        [BsonElement("NombreAlertes")]
        public int NombreAlertes { get; set; }

        [BsonElement("Numero")]
        public int Numero {  get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        public enum Tache
        {
            Semis,
            Rempotage,
            Entretien,
            Arrosage,
            Recolte,
            Commandes,
            Evenements
        }

        public Jour()
        {
            DateTime aujourdhui = DateTime.Now;
            TimeCreated = DateTime.Now;
            Taches = new ObservableCollection<Tache>();
            CommentaireTaches = new ObservableCollection<string>();
            NombreAlertes = 0;
            Numero = aujourdhui.Day;
            Date = aujourdhui.ToLocalTime();
        }

        public Jour(Jour jour)
        {
            TimeCreated = jour.Date;
            Taches = new ObservableCollection<Tache>(jour.Taches);
            CommentaireTaches = new ObservableCollection<string>(jour.CommentaireTaches);
            NombreAlertes = jour.NombreAlertes;
            Numero = jour.Numero;
            Date = jour.Date;
        }


        public Jour(DateTime date)
        {
            DateTime aujourdhui = DateTime.Now;
            TimeCreated = aujourdhui;
            Taches = new ObservableCollection<Tache>();
            CommentaireTaches = new ObservableCollection<string>();
            NombreAlertes = 0;
            Numero = date.Day;
            Date = date.ToLocalTime();
        }

		public Jour(ObservableCollection<Tache> taches, ObservableCollection<string> commentairesTaches, DateTime date)
		{
			DateTime aujourdhui = DateTime.Now;
			TimeCreated = aujourdhui;
			Taches = taches;
			CommentaireTaches = commentairesTaches;

			var traitementTacheAlertes = taches.Where(tache =>
			{
				int numeroTache = (int)tache;
				return numeroTache == 1 || numeroTache == 6;
			}).ToList();

			NombreAlertes = traitementTacheAlertes.Count;
			Numero = date.Day;
			Date = date.ToLocalTime();
		}

	}
}
