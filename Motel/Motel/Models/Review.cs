using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Review
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("sender")]
        [JsonPropertyName("sender")]
        public string Sender { get; set; } = null!;

        [BsonElement("sender_email")]
        [JsonPropertyName("sender_email")]
        public string SenderEmail { get; set; } = null!;

        [BsonElement("comment")]
        [JsonPropertyName("comment")]
        public string Comment { get; set; } = null!;

        [BsonElement("rating")]
        [JsonPropertyName("rating")]
        public int Rating { get; set; } = 1;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
