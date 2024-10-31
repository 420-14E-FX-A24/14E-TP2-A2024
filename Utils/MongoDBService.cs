using Automate.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Automate.Models.Jour;

namespace Automate.Utils
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<Jour> _jours;

        public MongoDBService(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017"); // URL du serveur MongoDB
            _database = client.GetDatabase(databaseName);
            _users = _database.GetCollection<UserModel>("Users");
            _jours = _database.GetCollection<Jour>("Jours");
            var premierUtilisateur = _users.Find(Builders<UserModel>.Filter.Empty).FirstOrDefault();
            if(premierUtilisateur is null)
            {
                premierUtilisateur = new UserModel { Username = "Frederic", Password = ".", Role = "Admin" };
                RegisterUser(premierUtilisateur);
            }
            var premierJour = _jours.Find(Builders<Jour>.Filter.Empty).FirstOrDefault();
            if (premierJour is null)
            {
                premierJour = new Jour();
                RegisterJour(premierJour);
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName) 
        {
            return _database.GetCollection<T>(collectionName);
        }

        public UserModel Authenticate(string? username, string? password)
        {
            var user = _users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return user;
        }

        public List<Jour> ConsulterJourCalendrierPage(DateTime date)
        {
            var Date = date;
            int JourDuMois = date.Day;
            var DepartJour = date.DayOfYear - JourDuMois;
            int FinJour = DepartJour + DateTime.DaysInMonth(Date.Year, Date.Month);
            var dateFiltre = Builders<Jour>.Filter.And(
                Builders<Jour>.Filter.Gte(doc => doc.TimeCreated.DayOfYear, DepartJour),
                Builders<Jour>.Filter.Lte(doc => doc.TimeCreated.DayOfYear, FinJour)
            );
            return _jours.Find(dateFiltre).ToList();
        }

        public void RegisterUser(UserModel user)
        {
            user.TimeCreated = DateTime.UtcNow;
            _users.InsertOne(user);
        }

        public void RegisterJour(Jour jour)
        {
            _jours.InsertOne(jour);
        }

    }

}
