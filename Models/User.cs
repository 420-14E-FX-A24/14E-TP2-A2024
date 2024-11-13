using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate.Models
{
    public class User
    {
        internal DateTime TimeUpdated;

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("Password")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("Role")]
        public Role Role { get; set; }
    }

    public enum Role
    {
        User,
        Admin
    }
}
