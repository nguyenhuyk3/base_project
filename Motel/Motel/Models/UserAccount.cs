using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using WebProject.Models;

namespace Motel.Models
{
    public class UserAccount
    {
        [BsonId]
        /*
         [BsonId] is an attribute in MongoDB.Driver used to specify that 
        a field in the C# class is the Id field of the MongoDB document.
        */
        [BsonElement("_id")]
        /*
        [BsonElement("name")] is an attribute 
        in MongoDB.Driver used to map 
        a field in a C# class to a field in a MongoDB document. 
        */
        [BsonRepresentation(BsonType.ObjectId)]
        /*
        [BsonRepresentation(BsonType.ObjectId)] is an attribute 
        in MongoDB.Driver used to specify 
        how a field is represented in a C# class when it is stored as 
        an ObjectId in MongoDB. 
        */
        [JsonPropertyName("id")]
        /*
        [JsonPropertyName("account_name")] is an attribute 
        in the System.Text.Json.Serialization namespace in .NET Core that 
        is used to specify the name of a field or 
        property when converting between a .NET object and JSON.
        */
        public string? Id { get; set; }

        [BsonElement("info")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("info")]
        public Info? Info { get; set; } = null;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        //[BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("role")]
        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;

        [BsonElement("rating")]
        [JsonPropertyName("rating")]
        public int Rating { get; set; } = 0;

        [BsonElement("balance")]
        [JsonPropertyName("balance")]
        public int Balance { get; set; } = 0;

        [BsonElement("posts")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("posts")]
        public List<Post>? Posts { get; set; } = null;

        [BsonElement("preference_list")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("preference_list")]
        public List<Post>? PreferenceList { get; set; } = null;

        [BsonElement("bills")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("bills")]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string>? Bills { get; set; } = null;

        // This field will save the reviews that users leave to rate 
        [BsonElement("active_reviews")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("active_reviews")]
        public List<Review>? ActiveReviews { get; set; } = null;

        // This field will save the reviews received by the user
        [BsonElement("passive_reviews")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("passive_reviews")]
        public List<Review>? PassiveReviews { get; set; } = null;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

