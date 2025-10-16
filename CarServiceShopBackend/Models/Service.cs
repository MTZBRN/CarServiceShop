using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models;

public class Service
{
    public Service(int id, string licensePlate, DateTime serviceDate, string serviceDescription, Car car, List<Part> parts)
    {
        Id = id;
        LicensePlate = licensePlate;
        ServiceDate = serviceDate;
        ServiceDescription = serviceDescription;
        Car = car;
        Parts = parts;
    }

    public Service()
    {
            
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public string LicensePlate { get; set; }
    public DateTime ServiceDate { get; set; }
    public string ServiceDescription { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public Car Car { get; set; }
    public List<Part> Parts { get; set; }
}