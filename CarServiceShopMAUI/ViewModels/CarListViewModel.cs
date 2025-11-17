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
        private ObservableCollection<Car> filteredCars = new();

        [ObservableProperty]
        private ObservableCollection<string> availableBrands = new();

        [ObservableProperty]
        private ObservableCollection<int> availableYears = new();

        [ObservableProperty]
        private string searchQuery = string.Empty;

        [ObservableProperty]
        private string? selectedBrand;

        [ObservableProperty]
        private int? selectedYear;

        [ObservableProperty]
        private bool isRefreshing;

        public IAsyncRelayCommand LoadCarsCommand { get; }
        public IAsyncRelayCommand AddCarCommand { get; }
        public IAsyncRelayCommand<Car> DeleteCarCommand { get; }
        public IAsyncRelayCommand<Car> EditCarCommand { get; }
        public IAsyncRelayCommand<Car> NavigateToServicesCommand { get; }
        public IRelayCommand ClearFiltersCommand { get; }

        public CarListPageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            LoadCarsCommand = new AsyncRelayCommand(LoadCarsAsync);
            AddCarCommand = new AsyncRelayCommand(AddCarAsync);
            DeleteCarCommand = new AsyncRelayCommand<Car>(DeleteCarAsync);
            EditCarCommand = new AsyncRelayCommand<Car>(EditCarAsync);
            NavigateToServicesCommand = new AsyncRelayCommand<Car>(NavigateToServicesAsync);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
        }

        partial void OnSearchQueryChanged(string value)
        {
            ApplyFilters();
        }

        partial void OnSelectedBrandChanged(string? value)
        {
            ApplyFilters();
        }

        partial void OnSelectedYearChanged(int? value)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filtered = Cars.AsEnumerable();

            // Keresés szöveg alapján
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                filtered = filtered.Where(car =>
                    car.LicensePlate.ToLower().Contains(query) ||
                    car.Brand.ToLower().Contains(query) ||
                    car.Model.ToLower().Contains(query));
            }

            // Márka szűrés
            if (!string.IsNullOrEmpty(SelectedBrand) && SelectedBrand != "Összes márka")
            {
                filtered = filtered.Where(car => car.Brand == SelectedBrand);
            }

            // Évjárat szűrés
            if (SelectedYear.HasValue && SelectedYear > 0)
            {
                filtered = filtered.Where(car => car.YearOfManufacture == SelectedYear);
            }

            FilteredCars.Clear();
            foreach (var car in filtered)
            {
                FilteredCars.Add(car);
            }
        }

        private void UpdateFilterOptions()
        {
            // Márkák listázása
            var brands = Cars.Select(c => c.Brand).Distinct().OrderBy(b => b).ToList();
            AvailableBrands.Clear();
            AvailableBrands.Add("Összes márka");
            foreach (var brand in brands)
            {
                AvailableBrands.Add(brand);
            }

            // Évjáratok listázása
            var years = Cars.Select(c => c.YearOfManufacture).Distinct().OrderByDescending(y => y).ToList();
            AvailableYears.Clear();
            AvailableYears.Add(0); // "Összes" opciónak
            foreach (var year in years)
            {
                AvailableYears.Add(year);
            }
        }

        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            SelectedBrand = null;
            SelectedYear = null;
        }

        private async Task LoadCarsAsync()
        {
            try
            {
                IsRefreshing = true;
                Debug.WriteLine("🔄 Loading cars from API...");

                var carsFromApi = await _apiService.GetCarsAsync();

                Cars.Clear();
                foreach (var car in carsFromApi)
                {
                    Cars.Add(car);
                }

                UpdateFilterOptions();
                ApplyFilters();

                Debug.WriteLine($"✅ Successfully loaded {Cars.Count} cars");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in LoadCarsAsync: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Nem sikerült betölteni az autókat: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task AddCarAsync()
        {
            Debug.WriteLine("➕ Navigating to add car page");
            await Shell.Current.GoToAsync(nameof(CarDetailPage));
        }

        private async Task DeleteCarAsync(Car? car)
        {
            if (car == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törölni szeretnéd ezt az autót: {car.LicensePlate}?",
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
                    ApplyFilters();
                    Debug.WriteLine("✅ Car deleted successfully");
                    await Shell.Current.DisplayAlert("Siker", "Autó sikeresen törölve!", "OK");
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

        private async Task EditCarAsync(Car? car)
        {
            if (car == null) return;

            Debug.WriteLine($"✏️ Navigating to edit car: {car.LicensePlate}");
            await Shell.Current.GoToAsync($"{nameof(CarDetailPage)}?CarId={car.Id}");
        }

        private async Task NavigateToServicesAsync(Car? car)
        {
            if (car == null) return;

            Debug.WriteLine($"🔧 Navigate to services for car: {car.LicensePlate}");
            await Shell.Current.GoToAsync($"{nameof(ServicePage)}?CarId={car.Id}");
        }
    }
}
