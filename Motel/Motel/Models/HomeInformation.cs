using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class HomeInformation
    {
        //[BsonElement("length")]
        //[JsonPropertyName("length")]
        //public float Length { get; set; } = 0;

        //[BsonElement("width")]
        //[JsonPropertyName("width")]
        //public float Width { get; set; } = 0;

        [BsonElement("square_meter")]
        [JsonPropertyName("square_meter")]
        public float SquareMeter { get; set; } = 0;

        //[BsonElement("furniture")]
        //[JsonPropertyName("furniture")]
        //public string Furniture { get; set; } = "Không nội thất";

        [BsonElement("bedroom")]
        [JsonPropertyName("bedroom")]
        public int Bedroom { get; set; } = 0;

        [BsonElement("toilet")]
        [JsonPropertyName("toilet")]
        public int Toilet { get; set; } = 0;

        [BsonElement("floor")]
        [JsonPropertyName("floor")]
        public int Floor { get; set; } = 0;
    }
}
