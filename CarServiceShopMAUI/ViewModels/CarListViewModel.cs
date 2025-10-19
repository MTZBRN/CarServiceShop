using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CarServiceShopMAUI.Services;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
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

        // Dependency injection constructor
        public CarListPageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
            AddCarCommand = new AsyncRelayCommand(AddCarAsync);
            DeleteCarCommand = new AsyncRelayCommand(DeleteCarAsync, CanModifyCar);
            EditCarCommand = new AsyncRelayCommand(EditCarAsync, CanModifyCar);
            NavigateToServicesCommand = new AsyncRelayCommand(NavigateToServicesAsync, CanModifyCar);
        }

        private bool CanModifyCar() => SelectedCar != null;

        partial void OnSelectedCarChanged(Car oldValue, Car newValue)
        {
            DeleteCarCommand.NotifyCanExecuteChanged();
            EditCarCommand.NotifyCanExecuteChanged();
            NavigateToServicesCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadCarsAsync()
        {
            try
            {
                Debug.WriteLine("🔄 Loading cars from API...");
                var carsFromApi = await _apiService.GetCarsAsync();
                var Cars = new ObservableCollection<Car>(carsFromApi);
                Debug.WriteLine($"✅ Successfully loaded {Cars.Count} cars");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in LoadCarsAsync: {ex.Message}");
            }
        }

        private async Task AddCarAsync()
        {
            // TODO: Implementáld a hozzáadási logikát
            Debug.WriteLine("➕ Add car clicked");
        }

        private async Task DeleteCarAsync()
        {
            if (SelectedCar == null) return;

            try
            {
                Debug.WriteLine($"🗑️ Deleting car: {SelectedCar.LicensePlate}");
                bool success = await _apiService.DeleteCarAsync(SelectedCar.Id);
                if (success)
                {
                    Cars.Remove(SelectedCar);
                    SelectedCar = null;
                    Debug.WriteLine("✅ Car deleted successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting car: {ex.Message}");
            }
        }

        private async Task EditCarAsync()
        {
            if (SelectedCar == null) return;
            Debug.WriteLine($"✏️ Edit car: {SelectedCar.LicensePlate}");
            // TODO: Implementáld a szerkesztési logikát
        }

        private async Task NavigateToServicesAsync()
        {
            if (SelectedCar == null) return;
            Debug.WriteLine($"🔧 Navigate to services for car: {SelectedCar.LicensePlate}");
            await Shell.Current.GoToAsync($"{nameof(ServicePage)}?CarId={SelectedCar.Id}");
        }
    }
}
