using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using System.IO;

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

        [ObservableProperty] private string brand = string.Empty;
        [ObservableProperty] private string model = string.Empty;
        [ObservableProperty] private string licensePlate = string.Empty;
        [ObservableProperty] private int yearOfManufacture = DateTime.Now.Year;
        [ObservableProperty] private DateTime dateOfTechnicalInspection = DateTime.Now.AddYears(1);
        [ObservableProperty] private string ownerName = string.Empty;
        [ObservableProperty] private string ownerAddress = string.Empty;
        [ObservableProperty] private string ownerPhone = string.Empty;
        [ObservableProperty] private string vin = string.Empty;
        [ObservableProperty] private int mileage;

        [ObservableProperty]
        private byte[]? imageData; // A mentéshez (byte[])

        [ObservableProperty]
        private bool isEdit;

        [ObservableProperty]
        private bool hasImage;

        [ObservableProperty]
        private ImageSource? imageSource;

        public IAsyncRelayCommand PickImageCommand { get; }
        public IAsyncRelayCommand TakePhotoCommand { get; }
        public IAsyncRelayCommand RemoveImageCommand { get; }
        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public CarDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;
            PickImageCommand = new AsyncRelayCommand(PickImageAsync);
            TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
            RemoveImageCommand = new AsyncRelayCommand(RemoveImageAsync);
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        // Automatikusan hívódik, ha változik az ImageData
        partial void OnImageDataChanged(byte[]? value)
        {
            if (value != null && value.Length > 0)
            {
                HasImage = true;
                ImageSource = ImageSource.FromStream(() => new MemoryStream(value));
            }
            else
            {
                HasImage = false;
                ImageSource = null;
            }
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
                var car = await _apiService.GetCarByIdAsync(id);
                if (car != null)
                {
                    Brand = car.Brand;
                    Model = car.Model;
                    LicensePlate = car.LicensePlate;
                    YearOfManufacture = car.YearOfManufacture;
                    DateOfTechnicalInspection = car.DateOfTechnicalInspection;
                    OwnerName = car.OwnerName ?? string.Empty;
                    OwnerAddress = car.OwnerAddress ?? string.Empty;
                    OwnerPhone = car.OwnerPhone ?? string.Empty;
                    Vin = car.Vin ?? string.Empty;
                    Mileage = car.Mileage;
                    ImageData = car.ImageData;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading car: {ex.Message}");
            }
        }

        // ✅ Gallériából kép választása
        private async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions { Title = "Kép kiválasztása" });
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ImageData = ms.ToArray(); // Trigger OnImageDataChanged
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni a képet! " + ex.Message, "OK");
            }
        }

        // ✅ Fotózás
        private async Task TakePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions { Title = "Fotó készítése" });
                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    ImageData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült fotót készíteni! " + ex.Message, "OK");
            }
        }

        // ✅ Kép törlése
        private async Task RemoveImageAsync()
        {
            ImageData = null;
            await Task.CompletedTask;
        }

        private async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Brand) || string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(LicensePlate))
                {
                    await Shell.Current.DisplayAlert("Hiba", "Márka, modell, rendszám kötelező!", "OK");
                    return;
                }

                var car = new Car
                {
                    Id = CarId,
                    Brand = Brand.Trim(),
                    Model = Model.Trim(),
                    LicensePlate = LicensePlate.Trim().ToUpper(),
                    YearOfManufacture = YearOfManufacture,
                    DateOfTechnicalInspection = DateOfTechnicalInspection,
                    OwnerName = OwnerName?.Trim(),
                    OwnerAddress = OwnerAddress?.Trim(),
                    OwnerPhone = OwnerPhone?.Trim(),
                    Vin = Vin?.Trim(),
                    Mileage = Mileage,
                    ImageData = ImageData
                };

                bool success;

                if (IsEdit && CarId > 0)
                    success = await _apiService.UpdateCarAsync(car);
                else
                    success = await _apiService.AddCarAsync(car);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Siker", IsEdit ? "Autó frissítve!" : "Autó hozzáadva!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
