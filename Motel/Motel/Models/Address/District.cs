﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace WebProject.Models.Address
{
    public class District
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("city_id")]
        [JsonPropertyName("city_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CityId { get; set; } = null!;

        [BsonElement("name")]
        [JsonPropertyName("name")]   
        public string Name { get; set; } = null!;
    }
}
