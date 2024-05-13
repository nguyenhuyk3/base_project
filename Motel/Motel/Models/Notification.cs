using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Notification
    {

        [BsonElement("sender")]
        [JsonPropertyName("sender")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Sender { get; set; } = null!;

        [BsonElement("content")]
        [JsonPropertyName("content")]
        public Motel.Areas.Customer.Models.Content Content { get; set; } = null!;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
