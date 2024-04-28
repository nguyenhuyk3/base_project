using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using WebProject.Models;

namespace WebProject.Models
{
    public class Post
    {
        [BsonId]
        [BsonElement("_id")]
        [JsonPropertyName("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("vip")]
        [JsonPropertyName("vip")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Vip { get; set; } = null!;

        [BsonElement("category")]
        [JsonPropertyName("category")]
        public string Category { get; set; } = null!;

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
        public PostDetail ContactInfo { get; set; } = null!;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [BsonElement("expired_at")]
        [JsonPropertyName("expired_at")]
        public DateTime ExpiredAt { get; set; }

        [BsonElement("booking")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("booking")]
        public List<Booking>? Booking { get; set; } = null;
    }
}
