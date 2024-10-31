using Automate.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Automate.Models.CalendrierPageModel;
using static Automate.Models.CalendrierPageModel.Jour;

namespace Automate.Utils
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _users;
        private readonly IMongoCollection<CalendrierPageModel> _calendrierPages;

        public MongoDBService(string databaseName)
        {
            var client = new MongoClient("mongodb://localhost:27017"); // URL du serveur MongoDB
            _database = client.GetDatabase(databaseName);
            _users = _database.GetCollection<UserModel>("Users");
            _calendrierPages = _database.GetCollection<CalendrierPageModel>("CalendrierPages");
            var premierUtilisateur = _users.Find(Builders<UserModel>.Filter.Empty).FirstOrDefault();
            if(premierUtilisateur is null)
            {
                premierUtilisateur = new UserModel { Username = "Frederic", Password = ".", Role = "Admin" };
                RegisterUser(premierUtilisateur);
            }
            var premierCalendrierPage = _calendrierPages.Find(Builders<CalendrierPageModel>.Filter.Empty).FirstOrDefault();
            if (premierCalendrierPage is null)
            {
                premierCalendrierPage = new CalendrierPageModel();
                RegisterCalendrierPage(premierCalendrierPage);
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

        public CalendrierPageModel ConsulterCalendrierPage(int annee, int mois)
        {
            var calendrierPage = _calendrierPages.Find(u => u.Annee == annee && u.Mois == mois).FirstOrDefault();
            return calendrierPage;
        }

        public void RegisterUser(UserModel user)
        {
            _users.InsertOne(user);
        }

        public void RegisterCalendrierPage(CalendrierPageModel calendrierPage)
        {
            _calendrierPages.InsertOne(calendrierPage);
        }

    }

}
