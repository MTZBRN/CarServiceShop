namespace CarServiceShopMAUI.Models;

public class Part
{
    public int Id { get; set; }
    public string PartNumber { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    
    public int ServiceId { get; set; }
    public Service Service { get; set; }
}