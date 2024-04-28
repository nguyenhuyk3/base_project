using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class Catergory
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [BsonElement("toggle")]
        [JsonPropertyName("toggle")]
        public bool IsToggled { get; set; } = true;
    }
}
