using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models.Address
{
    public class Award
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("district_id")]
        [JsonPropertyName("district_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DistrictId { get; set; } = null!;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
