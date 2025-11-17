using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(CarId), nameof(CarId))]
    public partial class CarDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private int carId;

        [ObservableProperty]
        private string pageTitle = "Új autó";

        [ObservableProperty]
        private string licensePlate = string.Empty;

        [ObservableProperty]
        private string brand = string.Empty;

        [ObservableProperty]
        private string model = string.Empty;

        [ObservableProperty]
        private int yearOfManufacture = DateTime.Now.Year;

        [ObservableProperty]
        private DateTime dateOfTechnicalInspection = DateTime.Now.AddYears(1);

        // Új mezők
        [ObservableProperty]
        private string ownerName = string.Empty;

        [ObservableProperty]
        private string ownerAddress = string.Empty;

        [ObservableProperty]
        private string ownerPhone = string.Empty;

        [ObservableProperty]
        private string vin = string.Empty;

        [ObservableProperty]
        private int mileage;

        [ObservableProperty]
        private bool isEdit;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public CarDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        partial void OnCarIdChanged(int value)
        {
            if (value > 0)
            {
                IsEdit = true;
                PageTitle = "Autó szerkesztése";
                _ = LoadCarAsync(value);
            }
            else
            {
                IsEdit = false;
                PageTitle = "Új autó";
            }
        }

        private async Task LoadCarAsync(int id)
        {
            try
            {
                Debug.WriteLine($"🔄 Loading car with ID: {id}");
                var car = await _apiService.GetCarByIdAsync(id);

                if (car != null)
                {
                    LicensePlate = car.LicensePlate;
                    Brand = car.Brand;
                    Model = car.Model;
                    YearOfManufacture = car.YearOfManufacture;
                    DateOfTechnicalInspection = car.DateOfTechnicalInspection;

                    OwnerName = car.OwnerName ?? string.Empty;
                    OwnerAddress = car.OwnerAddress ?? string.Empty;
                    OwnerPhone = car.OwnerPhone ?? string.Empty;
                    Vin = car.Vin ?? string.Empty;
                    Mileage = car.Mileage;

                    Debug.WriteLine($"✅ Car loaded: {car.LicensePlate}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading car: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                // Validáció
                if (string.IsNullOrWhiteSpace(LicensePlate))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A rendszám megadása kötelező!", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Brand))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A márka megadása kötelező!", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Model))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A modell megadása kötelező!", "OK");
                    return;
                }

                var car = new Car
                {
                    Id = CarId,
                    LicensePlate = LicensePlate.Trim(),
                    Brand = Brand.Trim(),
                    Model = Model.Trim(),
                    YearOfManufacture = YearOfManufacture,
                    DateOfTechnicalInspection = DateOfTechnicalInspection,
                    OwnerName = OwnerName.Trim(),
                    OwnerAddress = OwnerAddress.Trim(),
                    OwnerPhone = OwnerPhone.Trim(),
                    Vin = Vin.Trim(),
                    Mileage = Mileage
                };

                bool success;

                if (IsEdit)
                {
                    Debug.WriteLine($"✏️ Updating car: {car.LicensePlate}");
                    success = await _apiService.UpdateCarAsync(car);
                }
                else
                {
                    Debug.WriteLine($"➕ Adding new car: {car.LicensePlate}");
                    success = await _apiService.AddCarAsync(car);
                }

                if (success)
                {
                    Debug.WriteLine("✅ Save successful");
                    await Shell.Current.DisplayAlert("Siker",
                        IsEdit ? "Autó módosítva!" : "Autó hozzáadva!",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Debug.WriteLine("❌ Save failed");
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error saving car: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
