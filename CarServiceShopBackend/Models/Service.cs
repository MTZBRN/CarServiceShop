using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models;

public class Service
{
    public Service(int id, double workHours, double workHourPrice, DateTime serviceDate, string serviceDescription, Car car, List<Part> parts)
    {
        Id = id;
        WorkHours = workHours;
        WorkHourPrice = workHourPrice;
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
    
    public double WorkHours { get; set; }
    public double WorkHourPrice { get; set; }
    public DateTime ServiceDate { get; set; }
    public string ServiceDescription { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public List<Part>? Parts { get; set; }
}