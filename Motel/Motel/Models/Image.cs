using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class Image
    {
        [BsonElement("url")]
        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}