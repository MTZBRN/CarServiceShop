using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CarServiceShopMAUI.Models
{
    public class Car
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("licensePlate")]
        public string LicensePlate { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("yearOfManufacture")]
        public int YearOfManufacture { get; set; }

        [JsonPropertyName("dateOfTechnicalInspection")]
        public DateTime DateOfTechnicalInspection { get; set; }

        [JsonPropertyName("serviceJobs")]
        public List<Service> ServiceJobs { get; set; } = new();
    }
}
