using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Link
    {
        [BsonElement("area")]
        [JsonPropertyName("area")]
        public string Area { get; set; } = null!;

        [BsonElement("controller")]
        [JsonPropertyName("controller")]
        public string Controller { get; set; } = null!;

        [BsonElement("action")]
        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;
    }
}
