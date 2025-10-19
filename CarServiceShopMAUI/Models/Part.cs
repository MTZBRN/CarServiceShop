// File: `Models/Part.cs`
using System.Text.Json.Serialization;

namespace CarServiceShopMAUI.Models
{
    public class Part
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }

        [JsonPropertyName("service")]
        public string Service { get; set; }
    }
}