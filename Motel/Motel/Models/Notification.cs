using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class Notification
    {
        [BsonElement("sender_id")]
        [JsonPropertyName("sender_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SenderId { get; set; } = null!;

        [BsonElement("receiver_id")]
        [JsonPropertyName("receiver_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReceiverId { get; set; } = null!;

        [BsonElement("sender_img")]
        [JsonPropertyName("sender_img")]
        public string SenderImg { get; set; } = null!;

        [BsonElement("sender_full_name")]
        [JsonPropertyName("sender_full_name")]
        public string SenderFullName { get; set; } = null!;

        [BsonElement("rating")]
        [BsonIgnoreIfNull]
        [JsonPropertyName("rating")]
        public int? Rating { get; set; } = null;

        [BsonElement("content")]
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("is_readed")]
        [JsonPropertyName("is_readed")]
        public bool IsReaded { get; set; } = false;

        [BsonElement("link")]
        [JsonPropertyName("link")]
        public Link Link { get; set; } = null!;

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
