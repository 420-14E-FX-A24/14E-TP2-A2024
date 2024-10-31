using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using static Automate.Models.CalendrierPageModel.Jour;
using System.Windows.Media;

namespace Automate.Models
{
    public class CalendrierPageModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Annee")]
        public int Annee { get; set; }

        [BsonElement("Mois")]
        public int Mois { get; set; }

        [BsonElement("NombreSemaines")]
        public const int NombreSemaines = 5;

        [BsonElement("Jours")]
        public List<Jour> Jours { get; set; }



        public CalendrierPageModel()
        {
            DateTime DateAujourdhui = DateTime.Now;
            Annee = DateAujourdhui.Year;
            Mois = DateAujourdhui.Month;
            int JourDansMois = DateTime.DaysInMonth(DateAujourdhui.Year, DateAujourdhui.Month);
            Jours = new List<Jour>();
            for (int i = 1; i < JourDansMois; i++)
            {
                DateTime date = new DateTime(DateAujourdhui.Year, DateAujourdhui.Month, i);
                Jours.Add(new Jour(date));
            }

        }

        public CalendrierPageModel(int annee, int mois, List<Jour> jours)
        {
            Annee = annee;
            Mois = mois;
            int JourDansMois = DateTime.DaysInMonth(annee, mois);
            //Traitement de vérification/correction sur les jours.
            for (int i = 0; i < JourDansMois; i++)
            {
                DateTime date = new DateTime(annee, mois, i);
                if (jours.Count != JourDansMois)
                    jours.Add(new Jour(date));
                if (jours[i].Numero != i)
                  jours[i].Numero = i;
                List<Tache> traitementTacheAlertes = jours[i].Taches.FindAll(tache =>
                {
                    int numeroTache = (int)tache;
                    return numeroTache == 1 || numeroTache == 6;
                });
                if (jours[i].NombreAlertes != traitementTacheAlertes.Count)
                    jours[i].NombreAlertes = traitementTacheAlertes.Count;
                if (jours[i].JourSemaine is null || jours[i].JourSemaine != ((Journee)((int)date.DayOfWeek)))
                    jours[i].JourSemaine = ((Journee)((int)date.DayOfWeek));
            }
            Jours = jours;
        }


        public class Jour
        {
            [BsonElement("Taches")]
            public List<Tache> Taches { get; set; }

            [BsonElement("CommentairesTaches")]
            public List<string> CommentaireTaches { get; set; }

            [BsonElement("NombreAlertes")]
            public int NombreAlertes { get; set; }

            [BsonElement("JourSemaine")]
            public Journee? JourSemaine {  get; set; }

            [BsonElement("Numero")]
            public int Numero {  get; set; }

            public enum Journee
            {
                Dimanche,
                Lundi,
                Mardi,
                Mercredi,
                Jeudi,
                Vendredi,
                Samedi
            }

            public enum Tache
            {
                Semis,
                Rempotage,
                Desherbage,
                Taille,
                Fertilisation,
                Arrosage,
                Recolte,
                Commandes,
                visites,
                Evenementsspeciaux
            }

            public Jour()
            {
                DateTime DateAujourdhui = DateTime.Now;
                Taches = new List<Tache>();
                CommentaireTaches = new List<string>();
                NombreAlertes = 0;
                Numero = DateAujourdhui.Day;
                JourSemaine = ((Journee)((int)DateAujourdhui.DayOfWeek));
            }

            public Jour(DateTime date)
            {
                Taches = new List<Tache>();
                CommentaireTaches = new List<string>();
                NombreAlertes = 0;
                Numero = date.Day;
                JourSemaine = ((Journee)((int)date.DayOfWeek));
            }

            public Jour(List<Tache> taches, List<string> commentairesTaches, int numero)
            {
                Taches = taches;
                CommentaireTaches = commentairesTaches;
                List<Tache> traitementTacheAlertes = taches.FindAll(tache =>
                {
                    int numeroTache = (int)tache;
                    return numeroTache == 1 || numeroTache == 6;
                });
                NombreAlertes = traitementTacheAlertes.Count;
                Numero = numero;
            }


        }

    }
}
