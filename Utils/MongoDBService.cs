﻿using Automate.Models;
using MongoDB.Bson;
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

        public Jour ModifierJour(DateTime date)
        {
            DateTime queryDate = date.ToUniversalTime().Date;
            var premierJour = _jours.Find(Builders<Jour>.Filter.Empty).FirstOrDefault();
            var filter = Builders<Jour>.Filter.And(
                Builders<Jour>.Filter.Gte(j => j.Date, queryDate),
                Builders<Jour>.Filter.Lt(j => j.Date, queryDate.AddDays(1))
            );
            var result = _jours.Find(filter).FirstOrDefault();
            return result;
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
