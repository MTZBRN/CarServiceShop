// File: `Models/Part.cs`
using System.Text.Json.Serialization;

namespace CarServiceShopMAUI.Models
{
    public class Part
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("partNumber")]
        public string PartNumber { get; set; }

        [JsonPropertyName("netPrice")]
        public decimal NetPrice { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("serviceId")]
        public int ServiceId { get; set; }

        [JsonPropertyName("vatRate")]
        public decimal VATRate { get; set; }

        [JsonPropertyName("grossPrice")]
        public decimal GrossPrice { get; set; }


        [JsonIgnore]
        public Service Service { get; set; }
    }
}