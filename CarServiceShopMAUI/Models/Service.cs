using CommunityToolkit.Mvvm.ComponentModel;

namespace CarServiceShopMAUI.Models;

public partial class Service : ObservableObject
{
    [ObservableProperty]
    private int id;
    [ObservableProperty]
    private string description;
    [ObservableProperty]
    private DateTime date;
    [ObservableProperty]
    private decimal price;
    [ObservableProperty]
    private int workHours;
    [ObservableProperty]
    decimal workHourPrice;
    [ObservableProperty]
    private int carId;
    [ObservableProperty]
    Car car;
    [ObservableProperty]
    List<Part> parts;
    
}