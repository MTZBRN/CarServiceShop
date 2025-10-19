using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Views;
using System.Collections.ObjectModel;
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

        public CarListPageViewModel(ApiService apiService)
        {
            _apiService = apiService;

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
            try
            {
                Debug.WriteLine("🔄 Loading cars from API...");
                var carsFromApi = await _apiService.GetCarsAsync();

                Cars.Clear();
                foreach (var car in carsFromApi)
                {
                    Cars.Add(car);
                }

                Debug.WriteLine($"✅ Successfully loaded {Cars.Count} cars");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in LoadCarsAsync: {ex.Message}");
            }
        }

        private async Task AddCarAsync()
        {
            Debug.WriteLine("➕ Navigating to add car page");
            await Shell.Current.GoToAsync(nameof(CarDetailPage));
        }

        private async Task DeleteCarAsync()
        {
            if (SelectedCar == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törölni szeretnéd ezt az autót: {SelectedCar.LicensePlate}?",
                "Igen",
                "Nem");

            if (!confirm) return;

            try
            {
                Debug.WriteLine($"🗑️ Deleting car: {SelectedCar.LicensePlate}");
                bool success = await _apiService.DeleteCarAsync(SelectedCar.Id);

                if (success)
                {
                    Cars.Remove(SelectedCar);
                    SelectedCar = null;
                    Debug.WriteLine("✅ Car deleted successfully");
                    await Shell.Current.DisplayAlert("Siker", "Autó törölve!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült törölni az autót!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting car: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task EditCarAsync()
        {
            if (SelectedCar == null) return;

            Debug.WriteLine($"✏️ Navigating to edit car: {SelectedCar.LicensePlate}");
            await Shell.Current.GoToAsync($"{nameof(CarDetailPage)}?CarId={SelectedCar.Id}");
        }

        private async Task NavigateToServicesAsync()
        {
            if (SelectedCar == null) return;

            Debug.WriteLine($"🔧 Navigate to services for car: {SelectedCar.LicensePlate}");
            await Shell.Current.GoToAsync($"{nameof(ServicePage)}?CarId={SelectedCar.Id}");
        }
    }
}
