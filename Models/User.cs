using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Automate.Models
{
	public class User
	{
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