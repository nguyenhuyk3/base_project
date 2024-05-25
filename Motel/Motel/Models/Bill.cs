using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Bill
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [BsonElement("owner")]
        [JsonPropertyName("owner")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Owner { get; set; } = null!;

        [BsonElement("cost")]
        [JsonPropertyName("cost")]
        public int Cost { get; set; } = 0;

        [BsonElement("cost_string")]
        [JsonPropertyName("cost_string")]
        public string CostString { get; set; } = "0 VND";

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
