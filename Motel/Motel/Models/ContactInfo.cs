﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Motel.Models
{
    public class ContactInfo
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("owner")]
        [JsonPropertyName("owner")]
        public string? Owner { get; set; } = null;

        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [BsonElement("phone")]
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!;
    }
}
