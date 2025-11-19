using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

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
        private string brand = string.Empty;

        [ObservableProperty]
        private string model = string.Empty;

        [ObservableProperty]
        private string licensePlate = string.Empty;

        [ObservableProperty]
        private int year = DateTime.Now.Year;

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

        [ObservableProperty]
        private byte[] imageData;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }
        public IAsyncRelayCommand PickImageCommand { get; }

        public CarDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
            PickImageCommand = new AsyncRelayCommand(PickImageAsync);

            Debug.WriteLine("🏗️ CarDetailViewModel created");
        }

        // ✅ KRITIKUS: CarId alapján állítsd be az IsEdit-et!
        partial void OnCarIdChanged(int value)
        {
            Debug.WriteLine($"📝 CarId changed to: {value}");

            if (value > 0)
            {
                // ✅ Van ID → Szerkesztés
                IsEdit = true;
                PageTitle = "Autó szerkesztése";
                _ = LoadCarAsync(value);
            }
            else
            {
                // ✅ Nincs ID (0) → Új autó
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
                    Brand = car.Brand;
                    Model = car.Model;
                    LicensePlate = car.LicensePlate;
                    Year = car.YearOfManufacture;
                    OwnerName = car.OwnerName ?? string.Empty;
                    OwnerAddress = car.OwnerAddress ?? string.Empty;
                    OwnerPhone = car.OwnerPhone ?? string.Empty;
                    Vin = car.Vin ?? string.Empty;
                    Mileage = car.Mileage;
                    ImageData = car.ImageData;

                    Debug.WriteLine($"✅ Car loaded: {car.Brand} {car.Model}");
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
                Debug.WriteLine($"💾 Save started - CarId: {CarId}, IsEdit: {IsEdit}");

                // Validációk
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

                if (string.IsNullOrWhiteSpace(LicensePlate))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A rendszám megadása kötelező!", "OK");
                    return;
                }

                if (Year < 1900 || Year > DateTime.Now.Year + 1)
                {
                    await Shell.Current.DisplayAlert("Hiba", "Érvénytelen évjárat!", "OK");
                    return;
                }

                var car = new Car
                {
                    Id = CarId,  
                    Brand = Brand.Trim(),
                    Model = Model.Trim(),
                    LicensePlate = LicensePlate.Trim().ToUpper(),
                    YearOfManufacture = Year,
                    OwnerName = OwnerName?.Trim(),
                    OwnerAddress = OwnerAddress?.Trim(),
                    OwnerPhone = OwnerPhone?.Trim(),
                    Vin = Vin?.Trim(),
                    Mileage = Mileage,
                    ImageData = ImageData
                };

                Debug.WriteLine($"📦 Car object: Id={car.Id}, Brand={car.Brand}, IsEdit={IsEdit}");

                bool success;

                
                if (IsEdit && CarId > 0)
                {
                    Debug.WriteLine($"✏️ UPDATING car ID: {car.Id}");
                    success = await _apiService.UpdateCarAsync(car);
                }
                else
                {
                    Debug.WriteLine($"➕ ADDING new car: {car.Brand} {car.Model}");
                    success = await _apiService.AddCarAsync(car);
                }

                Debug.WriteLine($"🔍 API result: {success}");

                if (success)
                {
                    Debug.WriteLine("✅ Save successful!");
                    await Shell.Current.DisplayAlert("Siker",
                        IsEdit ? "Autó módosítva!" : "Autó hozzáadva!",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Debug.WriteLine("❌ Save failed - API returned false");
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ EXCEPTION in SaveAsync: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Válassz képet"
                });

                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    ImageData = memoryStream.ToArray();

                    Debug.WriteLine($"✅ Image picked: {ImageData.Length} bytes");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error picking image: {ex.Message}");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
