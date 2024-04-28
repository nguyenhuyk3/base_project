using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace WebProject.Models
{
    public class Booking
    {
        [BsonElement("months")]
        [JsonPropertyName("months")]
        public uint Months { get; set; } = 1;

        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; } = 0;

        [BsonElement("price_string")]
        [JsonPropertyName("price_string")]
        public string PriceString { get; set; } = "0 VND";

        [BsonElement("total_price")]
        [JsonPropertyName("total_price")]
        public decimal TotalPrice { get; set; } = 0;

        [BsonElement("total_price_string")]
        [JsonPropertyName("total_price_string")]
        public string TotalPriceString { get; set; } = "0 VND"!;

        [BsonElement("start_at")]
        [JsonPropertyName("start_at")]
        public DateTime StartAt { get; set; } = DateTime.Now;

        [BsonElement("end_at")]
        [JsonPropertyName("end_at")]
        public DateTime EndAt { get; set; }
    }
}
