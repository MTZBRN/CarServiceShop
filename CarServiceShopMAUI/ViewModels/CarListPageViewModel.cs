using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CarServiceShopMAUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace CarServiceShopMAUI.ViewModels
{
    public partial class CarListPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<Car> cars = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        // Row-alapú parancsok
        public IAsyncRelayCommand LoadCarsCommand { get; }
        public IAsyncRelayCommand AddCarCommand { get; }
        public IAsyncRelayCommand<Car> EditCarCommand { get; }
        public IAsyncRelayCommand<Car> DeleteCarCommand { get; }
        public IAsyncRelayCommand<Car> ViewServicesCommand { get; }
        public IAsyncRelayCommand<Car> CopyLicensePlateCommand { get; }

        public CarListPageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
            AddCarCommand = new AsyncRelayCommand(AddCarAsync);
            EditCarCommand = new AsyncRelayCommand<Car>(EditCarAsync);
            DeleteCarCommand = new AsyncRelayCommand<Car>(DeleteCarAsync);
            ViewServicesCommand = new AsyncRelayCommand<Car>(ViewServicesAsync);
            CopyLicensePlateCommand = new AsyncRelayCommand<Car>(CopyLicensePlateAsync);

            _ = LoadCarsAsync();
        }

        public async Task LoadCarsAsync()
        {
            try
            {
                Debug.WriteLine("🔄 Loading cars...");
                var carList = await _apiService.GetCarsAsync();

                Cars.Clear();
                foreach (var car in carList)
                {
                    Cars.Add(car);
                }

                Debug.WriteLine($"✅ Loaded {Cars.Count} cars");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading cars: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni az autókat!", "OK");
            }
        }

        private async Task AddCarAsync()
        {
            try
            {
                Debug.WriteLine("➕ Navigating to add car");
                await Shell.Current.GoToAsync(nameof(CarDetailPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to add car: {ex.Message}");
            }
        }

        private async Task EditCarAsync(Car car)
        {
            if (car == null)
            {
                Debug.WriteLine("⚠️ EditCarAsync: car is null");
                return;
            }

            try
            {
                Debug.WriteLine($"✏️ Editing car: {car.Brand} {car.Model} ({car.LicensePlate})");
                var navParams = new Dictionary<string, object>
                {
                    { "CarId", car.Id }
                };
                await Shell.Current.GoToAsync(nameof(CarDetailPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to edit car: {ex.Message}");
            }
        }

        private async Task DeleteCarAsync(Car car)
        {
            if (car == null)
            {
                Debug.WriteLine("⚠️ DeleteCarAsync: car is null");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törlöd ezt az autót?\n\n{car.Brand} {car.Model}\nRendszám: {car.LicensePlate}",
                "Igen",
                "Nem");

            if (!confirm) return;

            try
            {
                Debug.WriteLine($"🗑️ Deleting car: {car.LicensePlate}");
                bool success = await _apiService.DeleteCarAsync(car.Id);

                if (success)
                {
                    Cars.Remove(car);
                    Debug.WriteLine("✅ Car deleted successfully");
                    await Shell.Current.DisplayAlert("Siker", "Autó törölve!", "OK");
                }
                else
                {
                    Debug.WriteLine("❌ Delete failed - API returned false");
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült törölni az autót!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting car: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task ViewServicesAsync(Car car)
        {
            if (car == null)
            {
                Debug.WriteLine("⚠️ ViewServicesAsync: car is null");
                return;
            }

            try
            {
                Debug.WriteLine($"🔧 Navigating to services for car: {car.LicensePlate}");
                var navParams = new Dictionary<string, object>
                {
                    { "CarId", car.Id }
                };
                await Shell.Current.GoToAsync(nameof(ServicePage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to services: {ex.Message}");
            }
        }

        private async Task CopyLicensePlateAsync(Car car)
        {
            if (car == null || string.IsNullOrWhiteSpace(car.LicensePlate))
            {
                Debug.WriteLine("⚠️ CopyLicensePlateAsync: car or LicensePlate is null/empty");
                return;
            }

            try
            {
                Debug.WriteLine($"📋 Copying license plate: {car.LicensePlate}");

                await Clipboard.Default.SetTextAsync(car.LicensePlate);

                Debug.WriteLine("✅ License plate copied to clipboard");

                await Shell.Current.DisplayAlert(
                    "Másolva",
                    $"Rendszám vágólapra másolva:\n{car.LicensePlate}",
                    "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error copying license plate: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült másolni a rendszámot!", "OK");
            }
        }
    }
}