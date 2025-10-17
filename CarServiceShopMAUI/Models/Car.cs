using CommunityToolkit.Mvvm.ComponentModel;


namespace CarServiceShopMAUI.Models;

public partial class Car : ObservableObject
{
    

    [ObservableProperty] 
    int id;
    [ObservableProperty] 
    string licensePlate;
    [ObservableProperty] 
    private string brand;
    [ObservableProperty]
    private string model;
    [ObservableProperty]
    private int yearOfManufacture;
    [ObservableProperty] 
    DateTime dateOfTechnicalInspection;
    [ObservableProperty] 
    ICollection<Service> serviceJobs;


}