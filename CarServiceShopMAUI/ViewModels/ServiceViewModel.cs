using System.Collections.ObjectModel;
using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CarServiceShopMAUI.ViewModels;


public partial class ServiceListPageViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private int carId;

    [ObservableProperty]
    private ObservableCollection<Service> services = new();

    [ObservableProperty]
    private Service selectedService;

    [ObservableProperty]
    private ObservableCollection<Part> parts = new();

    public IAsyncRelayCommand LoadServicesCommand { get; }
    public IAsyncRelayCommand AddServiceCommand { get; }
    public IAsyncRelayCommand DeleteServiceCommand { get; }
    public IAsyncRelayCommand EditServiceCommand { get; }
    public IAsyncRelayCommand LoadPartsCommand { get; }

    public ServiceListPageViewModel()
    {
        _apiService = new ApiService();

        LoadServicesCommand = new AsyncRelayCommand(LoadServicesAsync);
        AddServiceCommand = new AsyncRelayCommand(AddServiceAsync);
        DeleteServiceCommand = new AsyncRelayCommand(DeleteServiceAsync, CanModifyService);
        EditServiceCommand = new AsyncRelayCommand(EditServiceAsync, CanModifyService);
        LoadPartsCommand = new AsyncRelayCommand(LoadPartsAsync, CanModifyService);
    }

    private bool CanModifyService()
    {
        return SelectedService != null;
    }

    partial void OnSelectedServiceChanged(Service oldValue, Service newValue)
    {
        DeleteServiceCommand.NotifyCanExecuteChanged();
        EditServiceCommand.NotifyCanExecuteChanged();
        LoadPartsCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadServicesAsync()
    {
        if (CarId == 0) return;
        var serviceList = await _apiService.GetServicesForCarAsync(CarId);
        Services = new ObservableCollection<Service>(serviceList);
    }

    private async Task AddServiceAsync()
    {
        // Implementáld a hozzáadás mechanizmust, például modalidad, form
    }

    private async Task DeleteServiceAsync()
    {
        if (SelectedService == null) return;

        bool success = await _apiService.DeleteServiceAsync(SelectedService.Id);
        if (success)
        {
            Services.Remove(SelectedService);
            SelectedService = null;
            Parts.Clear();
        }
    }

    private async Task EditServiceAsync()
    {
        if (SelectedService == null) return;
        // Implementáld szerkesztést
    }

    private async Task LoadPartsAsync()
    {
        if (SelectedService == null) return;

        var partsList = await _apiService.GetPartsForServiceAsync(SelectedService.Id);
        Parts = new ObservableCollection<Part>(partsList);
    }
}