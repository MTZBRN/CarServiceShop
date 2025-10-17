using CommunityToolkit.Mvvm.ComponentModel;

namespace CarServiceShopMAUI.Models;

public partial class Part : ObservableObject
{
    [ObservableProperty] 
    private int id;
    [ObservableProperty] 
    private string partNumber;
    [ObservableProperty] 
    private string description;
    [ObservableProperty] 
    private string name;
    [ObservableProperty] 
    private decimal price;
    [ObservableProperty] 
    private int quantity;
    [ObservableProperty] 
    private int serviceId;
    [ObservableProperty] 
    private Service service;
}