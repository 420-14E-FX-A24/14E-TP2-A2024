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

namespace Automate.Models
{

    public class Jour
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("timeCreated")]
        public DateTime TimeCreated { get; set; }

        [BsonElement("timeUpdated")]
        [BsonIgnoreIfNull]
        public DateTime TimeUpdated { get; set; }

        [BsonElement("Taches")]
        [BsonIgnoreIfNull]
        public List<Tache> Taches { get; set; }

        [BsonElement("CommentairesTaches")]
        [BsonIgnoreIfNull]
        public List<string> CommentaireTaches { get; set; }

        [BsonElement("NombreAlertes")]
        public int NombreAlertes { get; set; }

        [BsonElement("Numero")]
        public int Numero {  get; set; }


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
            TimeCreated = DateAujourdhui;
            Taches = new List<Tache>();
            CommentaireTaches = new List<string>();
            NombreAlertes = 0;
            Numero = DateAujourdhui.Day;
        }

        public Jour(DateTime date)
        {
            DateTime DateAujourdhui = DateTime.Now;
            TimeCreated = DateAujourdhui;
            Taches = new List<Tache>();
            CommentaireTaches = new List<string>();
            NombreAlertes = 0;
            Numero = date.Day;
        }

        public Jour(List<Tache> taches, List<string> commentairesTaches, int numero)
        {
            DateTime DateAujourdhui = DateTime.Now;
            TimeCreated = DateAujourdhui;
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
