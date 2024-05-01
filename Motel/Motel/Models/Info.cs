using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Info
    {
        [BsonElement("avatar")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; } = null;

        [BsonElement("full_name")]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = null!;

        [BsonElement("birth_day")]
        [JsonPropertyName("birth_day")]
        public DateTime BirthDay { get; set; }

        //[BsonElement("age")]
        //[JsonPropertyName("age")]
        //public uint Age { get; set; }

        [BsonElement("sex")]
        [JsonPropertyName("sex")]
        public bool Sex { get; set; }

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        [BsonIgnoreIfNull]
        public string? Phone { get; set; } = null;
    }
}
