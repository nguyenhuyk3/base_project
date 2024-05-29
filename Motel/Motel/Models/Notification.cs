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
        public string SenderId { get; set; } = null!;

        [BsonElement("full_name")]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = null!;

        [BsonElement("rating")]
        [JsonPropertyName("rating")]
        public int? Rating { get; set; } = null;

        [BsonElement("content")]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("is_readed")]
        [JsonPropertyName("is_readed")]
        public bool IsReaded { get; set; } = false;

        [BsonElement("link")]
        [JsonPropertyName("link")]
        public Link? Link { get; set; } = null;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
