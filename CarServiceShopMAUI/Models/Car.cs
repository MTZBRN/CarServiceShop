using System.Text.Json.Serialization;

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

    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; }

    [JsonPropertyName("ownerAddress")]
    public string OwnerAddress { get; set; }

    [JsonPropertyName("ownerPhone")]
    public string OwnerPhone { get; set; }

    [JsonPropertyName("vin")]
    public string Vin { get; set; }

    [JsonPropertyName("mileage")]
    public int Mileage { get; set; }
}
