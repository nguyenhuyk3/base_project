using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class Vip
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [BsonElement("token")]
        [JsonPropertyName("token")]
        public int Token { get; set; } = 0;

        [BsonElement("toggle")]
        [JsonPropertyName("toggle")]
        public bool Toggle { get; set; } = true;
    }
}
