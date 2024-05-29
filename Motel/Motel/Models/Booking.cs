using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Booking
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("owner")]
        [JsonPropertyName("owner")]
        public string Owner { get; set; } = null!;

        [BsonElement("contact_info")]
        [JsonPropertyName("contact_info")]
        public ContactInfo ContactInfo { get; set; } = null!;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("post_id")]
        [JsonPropertyName("post_id")]
        public string PostId { get; set; } = null!;

        [BsonElement("is_readed")]
        [JsonPropertyName("is_readed")]
        public bool IsReaded { get; set; } = false!;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
