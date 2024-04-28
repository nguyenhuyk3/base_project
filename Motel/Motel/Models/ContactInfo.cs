using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class ContactInfo
    {
        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!;
    }
}
