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
    public DateTime DateOfTechnicalInspection { get; set; }
    [NotMapped]
    public ICollection<Service> ServiceJobs { get; set; }

    public Car(int carId, string licensePlate, string brand, string model, int yearOfManufacture,DateTime DateOfTechnicalInspection, ICollection<Service> services)
    {
        Id = carId;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        YearOfManufacture = yearOfManufacture;
        this.DateOfTechnicalInspection = DateOfTechnicalInspection;
        ServiceJobs = services;
    }
    
    public Car()
    {
    }
}