using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MongoDB.Driver;

namespace WebProject.ViewModels
{
    public class PostViewModel
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PostId { get; set; }

        [BsonElement("category")]
        [JsonPropertyName("category")]
        public string Category { get; set; } = null!;

        [BsonElement("subject")]
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = null!;

        [BsonElement("address_string")]
        [JsonPropertyName("address_string")]
        public string AddressString { get; set; } = null!;

        [BsonElement("city")]
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        [BsonElement("district")]
        [JsonPropertyName("district")]
        public string District { get; set; } = null!;

        [BsonElement("award")]
        [JsonPropertyName("award")]
        public string Award { get; set; } = null!;

        [BsonElement("street")]
        [JsonPropertyName("street")]
        public string Street { get; set; } = null!;
        [BsonElement("length")]
        [JsonPropertyName("length")]
        public float Length { get; set; } = 0;

        [BsonElement("views")]
        [JsonPropertyName("views")]
        public uint Views { get; set; } = 0;

        [BsonElement("width")]
        [JsonPropertyName("width")]
        public float Width { get; set; } = 0;

        [BsonElement("square_meter")]
        [JsonPropertyName("square_meter")]
        public float SquareMeter { get; set; } = 0;

        [BsonElement("bed_room")]
        [JsonPropertyName("bed_room")]
        public uint BedRoom { get; set; } = 0;

        [BsonElement("toilet")]
        [JsonPropertyName("toilet")]
        public uint Toilet { get; set; } = 0;

        [BsonElement("floor")]
        [JsonPropertyName("floor")]
        public uint Floor { get; set; } = 0;

        [BsonElement("number_of_image")]
        [JsonPropertyName("number_of_image")]
        public uint NumberOfImage { get; set; } = 0;

        [BsonElement("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; } = 0;

        [BsonElement("price_string")]
        [JsonPropertyName("price_string")]
        public string PriceString { get; set; } = "0 VND";
    }
}
