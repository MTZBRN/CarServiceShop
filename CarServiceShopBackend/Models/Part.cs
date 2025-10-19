using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServiceShopBackend.Models;

public class Part
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string PartNumber { get; set; } // Pl.: "123-ABC"
    [Required]
    public string Name { get; set; } // Pl.: "Stabilizátor"
    public double Price { get; set; } // Pl.: 1500.00
    public int Quantity { get; set; } // Pl.: 2
    
    public string Description { get; set; } // Pl.: "Stabilizátor pálca"

    [ForeignKey("Service")]
    public int ServiceId { get; set; }

    public Service Service { get; set; }

    public Part(int id, string partNumber, string name, double price, int quantity, string description, int serviceId)
    {
        Id = id;
        PartNumber = partNumber;
        Name = name;
        Price = price;
        Quantity = quantity;
        Description = description;
        ServiceId = serviceId;
    }
    public Part()
    {
        
    }
}