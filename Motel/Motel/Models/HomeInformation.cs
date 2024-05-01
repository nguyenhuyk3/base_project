using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class HomeInformation
    {
        [BsonElement("length")]
        [JsonPropertyName("length")]
        public float Length { get; set; } = 0;

        [BsonElement("width")]
        [JsonPropertyName("width")]
        public float Width { get; set; } = 0;

        [BsonElement("square_meter")]
        [JsonPropertyName("square_meter")]
        public float SquareMeter { get; set; } = 0;

        [BsonElement("bedroom")]
        [JsonPropertyName("bedroom")]
        public uint Bedroom { get; set; } = 0;

        [BsonElement("toilet")]
        [JsonPropertyName("toilet")]
        public uint Toilet { get; set; } = 0;

        [BsonElement("floor")]
        [JsonPropertyName("floor")]
        public uint Floor { get; set; } = 0;
    }
}
