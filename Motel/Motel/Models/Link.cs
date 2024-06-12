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
        [BsonIgnoreIfNull]
        [BsonElement("owner_id")]
        [JsonPropertyName("owner_id")]
        public string? OwnerId { get; set; } = null;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        [BsonElement("post_id")]
        [JsonPropertyName("post_id")]
        public string? PostId { get; set; } = null;
    }
}
