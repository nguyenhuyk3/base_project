using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models.Address
{
    public class Street
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("award_id")]
        [JsonPropertyName("award_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AwardId { get; set; } = null!;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
