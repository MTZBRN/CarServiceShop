namespace CarServiceShopMAUI.Models;

public class Service
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public int workHours { get; set; }
    public decimal workHourPrice { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }
    public List<Part> Parts { get; set; }
}