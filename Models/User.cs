using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate.Models
{
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Username")]
        public string? Username { get; set; }

        [BsonElement("Password")]
        public string? Password { get; set; }

        [BsonElement("Role")]
        public string? Role { get; set; }
    }
}
