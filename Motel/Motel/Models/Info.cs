using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class Info
    {
        [BsonElement("avatar")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; } = null;

        [BsonElement("full_name")]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = null!;

        [BsonElement("age")]
        [JsonPropertyName("age")]
        public uint Age { get; set; }

        [BsonElement("sex")]
        [JsonPropertyName("sex")]
        public string Sex { get; set; } = null!;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!;
    }
}
