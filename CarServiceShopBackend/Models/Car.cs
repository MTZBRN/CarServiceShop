using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models;

public class Car
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string LicensePlate { get; set; }
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Model { get; set; }
    public int YearOfManufacture { get; set; }
    public int Mileage { get; set; }
    public string Vin { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public string OwnerPhone { get; set; }
    public byte[]? picture { get; set; } = null;



    public DateTime DateOfTechnicalInspection { get; set; }
    [NotMapped]
    public ICollection<Service>? ServiceJobs { get; set; }
    
    public Car()
    {
    }

    public Car(int id, string licensePlate, string brand, string model, int yearOfManufacture, int mileage, string vin, string ownerName, string ownerAddress, string ownerPhone, byte[] picture)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        YearOfManufacture = yearOfManufacture;
        Mileage = mileage;
        Vin = vin;
        OwnerName = ownerName;
        OwnerAddress = ownerAddress;
        OwnerPhone = ownerPhone;
        this.picture = picture;
    }
}