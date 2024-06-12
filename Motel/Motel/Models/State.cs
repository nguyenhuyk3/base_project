using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class State
    {
        [BsonElement("is_violated")]
        [JsonPropertyName("is_violated")]
        public bool IsViolated { get; set; } = false;

        [BsonElement("is_toggled")]
        [JsonPropertyName("is_toggled")]
        public bool IsToggled { get; set; } = true;

        [BsonElement("is_edited")]
        [JsonPropertyName("is_edited")]
        public bool IsEdited { get; set; } = false;

        [BsonElement("is_deleted")]
        [JsonPropertyName("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [BsonElement("is_authenticated")]
        [JsonPropertyName("is_authenticated")]
        public bool IsAuthenticated { get; set; } = false;
    }
}
