using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class AddressDetail
    {
        [BsonElement("address")]
        [JsonPropertyName("address")]
        public string Address { get; set; } = null!;

        [BsonElement("city")]
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        [BsonElement("district")]
        [JsonPropertyName("district")]
        public string District { get; set; } = null!;

        [BsonElement("ward")]
        [JsonPropertyName("ward")]
        public string Ward { get; set; } = null!;

        [BsonElement("street")]
        [JsonPropertyName("street")]
        public string Street { get; set; } = null!;

        [BsonElement("longitude")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("longitude")]
        public string? Longitude { get; set; } = null;

        [BsonElement("laitude")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("laitude")]
        public string? Latitude { get; set; } = null;
    }
}
