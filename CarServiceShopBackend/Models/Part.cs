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
    public string Description { get; set; } // Pl.: "Stabilizátor pálca"

    [ForeignKey("Service")]
    public int ServiceId { get; set; }

    public Service Service { get; set; }
}