using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models.Address
{
    public class City
    {
        //[BsonId]
        //[BsonElement("id")]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Id { get; set; }

        //[BsonElement("name")]
        //[JsonPropertyName("name")]
        //public string Name { get; set; } = null!;

        //[BsonElement("api_id")]
        //[JsonPropertyName("api_id")]
        //public int ApiId { get; set; } = 0;

        //[BsonElement("is_toggled")]
        //[JsonPropertyName("is_toggled")]
        //public bool IsToggled { get; set; } = true; 

        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [BsonElement("province_id")]
        [JsonPropertyName("province_id")]
        public string ProvinceId { get; set; } = null!;

        [BsonElement("is_toggled")]
        [JsonPropertyName("is_toggled")]
        public bool IsToggled { get; set; } = true;
    }
}
