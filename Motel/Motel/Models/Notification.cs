using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using MongoDB.Driver.Core.Configuration;

namespace Motel.Models
{
    public class Notification
    {

        [BsonElement("sender")]
        [JsonPropertyName("sender")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Sender { get; set; } = null!;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("rating")]
        [JsonPropertyName("rating")]
        public int Rating { get; set; } = 0;

        [BsonElement("comment")]
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("is_readed")]
        [JsonPropertyName("is_readed")]
        public bool IsReaded { get; set; } = false;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
