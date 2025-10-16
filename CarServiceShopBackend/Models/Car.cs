using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models;

public class Car
{
    [Key]
    public int CarId { get; set; }
    [Required]
    public string LicensePlate { get; set; }
    [Required]
    public string Brand { get; set; }
    [Required]
    public string Model { get; set; }
    public DateTime DateOfTechnicalInspection { get; set; }
    [NotMapped]
    public ICollection<Service> ServiceJobs { get; set; }

    public Car(int carId, string licensePlate, string brand, string model, DateTime DateOfTechnicalInspection, ICollection<Service> services)
    {
        CarId = carId;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        this.DateOfTechnicalInspection = DateOfTechnicalInspection;
        ServiceJobs = services;
    }
    
    public Car()
    {
    }
}