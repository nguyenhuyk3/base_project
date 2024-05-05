using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class PostDetail
    {
        //[BsonElement("subject_on_site")]
        //[JsonPropertyName("subject_on_site")]
        //public string SubjectOnSite { get; set; } = null!;

        [BsonElement("address_detail")]
        [JsonPropertyName("address_detail")]
        public AddressDetail AddressDetail { get; set; } = null!;

        [BsonElement("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [BsonElement("home_information")]
        [JsonPropertyName("home_information")]
        public HomeInformation HomeInformation { get; set; } = null!;

        [BsonElement("images")]
        [JsonPropertyName("images")]
        public List<Image> Images { get; set; } = null!;

        [BsonElement("number_of_image")]
        [JsonPropertyName("number_of_image")]
        public int NumberOfImage { get; set; } = 0;

        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; } = 0;

        [BsonElement("price_string")]
        [JsonPropertyName("price_string")]
        public string PriceString { get; set; } = "0 VND";

        [BsonElement("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [BsonElement("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
