using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CarServiceShopMAUI.Models
{
    public class Service
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("workHours")]
        public double WorkHours { get; set; }

        [JsonPropertyName("workHourPrice")]
        public double WorkHourPrice { get; set; }

        [JsonPropertyName("serviceDate")]
        public DateTime ServiceDate { get; set; }

        [JsonPropertyName("serviceDescription")]
        public string ServiceDescription { get; set; }
        public double TotalPrice { get; set; }

        [JsonPropertyName("carId")]
        public int CarId { get; set; }

        [JsonPropertyName("car")]
        public string Car { get; set; }

        [JsonPropertyName("parts")]
        public List<Part> Parts { get; set; } = new();
    }
}