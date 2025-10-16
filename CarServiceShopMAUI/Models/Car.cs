namespace CarServiceShopMAUI.Models;

public class Car
{
    public Car(int id, string licensePlate, string brand, string model, int yearOfManufacture, DateTime dateOfTechnicalInspection, ICollection<Service> serviceJobs)
    {
        Id = id;
        LicensePlate = licensePlate;
        Brand = brand;
        Model = model;
        YearOfManufacture = yearOfManufacture;
        DateOfTechnicalInspection = dateOfTechnicalInspection;
        ServiceJobs = serviceJobs;
    }

    public int Id { get; set; }
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int YearOfManufacture { get; set; }
    public DateTime DateOfTechnicalInspection { get; set; }
    public ICollection<Service> ServiceJobs { get; set; }

    
}