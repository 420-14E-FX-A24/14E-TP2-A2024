using Automate.Models;
using Microsoft.Extensions.Primitives;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using static Automate.Models.Jour;

namespace Automate.Utils
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Jour> _jours;

		public MongoDBService(string databaseName)
		{
			var client = new MongoClient("mongodb://localhost:27017");
			_database = client.GetDatabase(databaseName);
			_users = _database.GetCollection<User>("Users");
			_jours = _database.GetCollection<Jour>("Jours");

			AjoutPremierUtilisateur(Role.User, "Andre");
			AjoutPremierUtilisateur(Role.Admin, "Frederic");
		}

		private void AjoutPremierUtilisateur(Role role, string username)
		{
            var user = FindUserRoleFirstOrDefault(role);

            if (user is null) 
                RegisterUser(new User { Username = username, PasswordHash = "$2a$11$Rc0K8jktZrVizcxsNmEQU.c94VWEHjKxrmk0I09p5dkBteMSoJ2Bq", Role = role });
		}

		private User? FindUserRoleFirstOrDefault(Role role)
        {
            var filter = Builders<User>.Filter.Eq(user => user.Role, role);
            return _users.Find(filter).FirstOrDefault();
        }

		public User? Authenticate(string? username, string? password)
		{
			var user = _users.Find(u => u.Username == username).FirstOrDefault();
			if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			{
				return user;
			}
			return null;
		}

		public List<Jour> ConsulterJourCalendrierPage(DateTime date)
        {
            
            DateTime DepartJour = new DateTime(date.Year, date.Month, 1);
            DateTime FinJour = DepartJour.AddMonths(1).AddDays(-1);

            int PremierJourAnnee = DepartJour.DayOfYear;
            int DernierJourAnnee = FinJour.DayOfYear;

            var filter = new BsonDocument("$expr", new BsonDocument("$and", new BsonArray
            {
                new BsonDocument("$gte", new BsonArray { new BsonDocument("$dayOfYear", "$Date"), PremierJourAnnee }),
                new BsonDocument("$lte", new BsonArray { new BsonDocument("$dayOfYear", "$Date"), DernierJourAnnee })
            }));

            return _jours.Find(filter).ToList();  
        }


        public Jour? ConsulterJour(DateTime date)
        {
			var filter = Builders<Jour>.Filter.Eq(j => j.Numero, date.Day);
			return _jours.Find(filter).FirstOrDefault();
        }

        public void RegisterUser(User user)
        {
            _users.InsertOne(user);
        }

        public void RegisterJour(Jour jour)
        {
            _jours.InsertOne(jour);
        }



		public void EnregistrerModificationTache(Tache tache, int index, Jour jour)
		{
			if (index < 0 || index >= jour.Taches.Count) return;

			var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
			var update = Builders<Jour>.Update.Set(j => j.Taches[index], tache);

			_jours.UpdateOne(filter, update);
		}

		public void EnregistrerModificationCommentaire(string commentaire, int index, Jour jour)
        {
			if (index < 0 || index >= jour.Taches.Count) return;

			var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
			var update = Builders<Jour>.Update.Set(j => j.CommentaireTaches[index], commentaire);

			_jours.UpdateOne(filter, update);
		}

        public void EnregistrerAjoutTache(Tache tache, Jour jour)
        {
            if(jour.Id.ToString() == "000000000000000000000000")
            {
                RegisterJour(jour);
            }
            else
            {
                var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
                var update = Builders<Jour>.Update.Push(j => j.Taches, tache);
                _jours.UpdateOne(filter, update);
            }
        }

        public void EnregistrerAjoutCommentaire(string commentaire, Jour jour)
        {
            var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
            var update = Builders<Jour>.Update.Push(j => j.CommentaireTaches, commentaire);
            _jours.UpdateOne(filter, update);
        }

        public void EnregistrerRetraitTache(Tache tache, Jour jour)
        {
            var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
            var update = Builders<Jour>.Update.Pull(j => j.Taches, tache);
            _jours.UpdateOne(filter, update);
        }

        public void EnregistrerRetraitCommentaire(string commentaire, Jour jour)
        {
            var filter = Builders<Jour>.Filter.Eq(j => j.Id, jour.Id);
            var update = Builders<Jour>.Update.Pull(j => j.CommentaireTaches, commentaire);
            _jours.UpdateOne(filter, update);
        }

    }

}
