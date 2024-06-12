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

        // public ObjectId 
        public string? Id { get; set; }

        [BsonElement("reset_token")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("reset_token")]
        public string? ResetToken { get; set; } = null;


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

        [BsonElement("role_name")]
        [JsonPropertyName("role_name")]
        public string RoleName { get; set; } = null!;

        [BsonElement("total_rating")]
        [JsonPropertyName("total_rating")]
        [BsonIgnoreIfNull]
        public int TotalRating { get; set; } = 0;

        [BsonElement("rating")]
        [JsonPropertyName("rating")]
        public float Rating { get; set; } = 0;

        [BsonElement("balance")]
        [JsonPropertyName("balance")]
        public int Balance { get; set; } = 0;

        [BsonElement("posts")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("posts")]
        public List<string>? Posts { get; set; } = null;

        [BsonElement("favorite_list")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("favorite_list")]
        public List<string>? FavoriteList { get; set; } = null;

        [BsonElement("bills")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("bills")]
        public List<Bill>? Bills { get; set; } = null;

        // This field will save persons who owner rate them
        [BsonElement("active_review_persons")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("active_review_persons")]
        public List<string>? ActiveReviewPersons { get; set; } = null;

        // This field will save persons who rate myself
        [BsonElement("passive_review_persons")]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("passive_review_persons")]
        public List<string>? PassiveReviewPersons { get; set; } = null;

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

        [BsonElement("notifications")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("notifications")]
        public List<Notification>? Notifications { get; set; } = null;

        [BsonElement("bookings")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("bookings")]
        public List<Booking>? Bookings { get; set; } = null;

        //[BsonElement("readed_notification")]
        //[BsonIgnoreIfNull]
        //[JsonPropertyName("readed_notification")]
        //public List<Notification>? ReadedNotifications { get; set; } = null;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

