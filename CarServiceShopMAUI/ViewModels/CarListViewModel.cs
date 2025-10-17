using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarServiceShopMAUI.Services;

public partial class CarListPageViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Car> cars = new();

    [ObservableProperty]
    private Car selectedCar;

    public IAsyncRelayCommand LoadCarsCommand { get; }
    public IAsyncRelayCommand AddCarCommand { get; }
    public IAsyncRelayCommand DeleteCarCommand { get; }
    public IAsyncRelayCommand EditCarCommand { get; }
    public IAsyncRelayCommand NavigateToServicesCommand { get; }

    public CarListPageViewModel()
    {
        _apiService = new ApiService();

        LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
        AddCarCommand = new AsyncRelayCommand(AddCarAsync);
        DeleteCarCommand = new AsyncRelayCommand(DeleteCarAsync, CanModifyCar);
        EditCarCommand = new AsyncRelayCommand(EditCarAsync, CanModifyCar);
        NavigateToServicesCommand = new AsyncRelayCommand(NavigateToServicesAsync, CanModifyCar);
    }

    private bool CanModifyCar()
    {
        return SelectedCar != null;
    }

    partial void OnSelectedCarChanged(Car oldValue, Car newValue)
    {
        DeleteCarCommand.NotifyCanExecuteChanged();
        EditCarCommand.NotifyCanExecuteChanged();
        NavigateToServicesCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadCarsAsync()
    {
        var carsFromApi = await _apiService.GetCarsAsync();
        Cars = new ObservableCollection<Car>(carsFromApi);
    }

    private async Task AddCarAsync()
    {
        // Implementáld a hozzáadási logikát, pl. modális oldal, popup form
    }

    private async Task DeleteCarAsync()
    {
        if (SelectedCar == null) return;

        bool success = await _apiService.DeleteCarAsync(SelectedCar.Id);
        if (success)
        {
            Cars.Remove(SelectedCar);
            SelectedCar = null;
        }
    }

    private async Task EditCarAsync()
    {
        if (SelectedCar == null) return;

        // Implementáld a szerkesztési logikát, pl. modális oldal, form
    }

    private async Task NavigateToServicesAsync()
    {
        if (SelectedCar == null) return;

        await Shell.Current.GoToAsync($"{nameof(ServicePage)}?CarId={SelectedCar.Id}");
    }
}
