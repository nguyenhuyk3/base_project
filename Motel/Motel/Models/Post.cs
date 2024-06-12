using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Post
    {
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("owner_id")]
        [JsonPropertyName("owner_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OwnerId { get; set; } = null!;

        [BsonElement("vip_name")]
        [JsonPropertyName("vip_name")]
        public string VipName { get; set; } = null!;

        [BsonElement("category_name")]
        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; } = null!;

        [BsonElement("subject_on_site")]
        [JsonPropertyName("subject_on_site")]
        public string SubjectOnSite { get; set; } = null!;

        [BsonElement("state")]
        [JsonPropertyName("state")]
        public State State { get; set; } = null!;

        [BsonElement("views")]
        [JsonPropertyName("views")]
        public uint Views { get; set; } = 0;

        [BsonElement("post_detail")]
        [JsonPropertyName("post_detail")]
        public PostDetail PostDetail { get; set; } = null!;

        [BsonElement("contact_info")]
        [JsonPropertyName("contact_info")]
        public ContactInfo ContactInfo { get; set; } = null!;

        [BsonElement("bookings")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("bookings")]
        public List<Booking>? Bookings { get; set; } = null;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [BsonElement("expired_at")]
        [JsonPropertyName("expired_at")]
        public DateTime ExpiredAt { get; set; }
    }
}
