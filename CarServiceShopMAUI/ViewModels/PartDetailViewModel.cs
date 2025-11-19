using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(PartId), nameof(PartId))]
    [QueryProperty(nameof(ServiceId), nameof(ServiceId))]

    public partial class PartDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private int partId;

        [ObservableProperty]
        private string pageTitle = "Új alkatrész";

        [ObservableProperty]
        private string partNumber = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private int quantity = 1;

        [ObservableProperty]
        private decimal netPrice;

        [ObservableProperty]
        private decimal vatRate = 0.27m;

        [ObservableProperty]
        private int serviceId;

        public decimal GrossPrice => NetPrice * (1 + VatRate);

        public decimal TotalPrice => GrossPrice * Quantity;

        [ObservableProperty]
        private bool isEdit;

        partial void OnQuantityChanged(int value) { OnPropertyChanged(nameof(TotalPrice)); }
        partial void OnNetPriceChanged(decimal value) { OnPropertyChanged(nameof(GrossPrice)); OnPropertyChanged(nameof(TotalPrice)); }
        partial void OnVatRateChanged(decimal value) { OnPropertyChanged(nameof(GrossPrice)); OnPropertyChanged(nameof(TotalPrice)); }


        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public PartDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        partial void OnPartIdChanged(int value)
        {
            if (value > 0)
            {
                IsEdit = true;
                PageTitle = "Alkatrész szerkesztése";
                _ = LoadPartAsync(value);
            }
            else
            {
                IsEdit = false;
                PageTitle = "Új alkatrész";
            }
        }

        private async Task LoadPartAsync(int id)
        {
            try
            {
                Debug.WriteLine($"🔄 Betöltés: alkatrész ID={id}");
                var part = await _apiService.GetPartByIdAsync(id);
                if (part != null)
                {
                    PartNumber = part.PartNumber;
                    Name = part.Name;
                    Description = part.Description;
                    Quantity = part.Quantity;
                    NetPrice = part.NetPrice;
                    VatRate = part.VATRate;
                    ServiceId = part.ServiceId;

                    Debug.WriteLine($"✅ Alkatrész betöltve: {part.Name}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Betöltési hiba: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                Debug.WriteLine($"💾 Save started - ServiceId: {ServiceId}, PartId: {PartId}");

                if (ServiceId == 0)
                {
                    Debug.WriteLine("❌ ERROR: ServiceId is 0!");
                    await Shell.Current.DisplayAlert("Hiba",
                        "Hiányzik a szerviz azonosító! Kérlek, térj vissza és próbáld újra.",
                        "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PartNumber))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A cikkszám megadása kötelező!", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert("Hiba", "Az alkatrész neve kötelező!", "OK");
                    return;
                }

                if (NetPrice <= 0)
                {
                    await Shell.Current.DisplayAlert("Hiba", "A nettó árnak nagyobbnak kell lennie 0-nál!", "OK");
                    return;
                }

                if (Quantity <= 0)
                {
                    await Shell.Current.DisplayAlert("Hiba", "A mennyiségnek nagyobbnak kell lennie 0-nál!", "OK");
                    return;
                }

                var part = new Part
                {
                    Id = PartId,
                    ServiceId = ServiceId,
                    PartNumber = PartNumber.Trim(),
                    Name = Name.Trim(),
                    Description = Description?.Trim() ?? string.Empty,
                    Quantity = Quantity,
                    NetPrice = NetPrice,
                    VATRate = VatRate,
                    GrossPrice = GrossPrice,
                    Service = new Service {Id = ServiceId }
                };

                Debug.WriteLine($"📦 Part object: ServiceId={part.ServiceId}, Name={part.Name}, NetPrice={part.NetPrice}");

                bool success = IsEdit
                    ? await _apiService.UpdatePartAsync(part)
                    : await _apiService.AddPartAsync(part);

                Debug.WriteLine($"🔍 API result: {success}");

                if (success)
                {
                    Debug.WriteLine("✅ Save successful!");
                    await Shell.Current.DisplayAlert("Siker",
                        IsEdit ? "Alkatrész módosítva!" : "Alkatrész hozzáadva!",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Debug.WriteLine("❌ Save failed - API returned false");
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni! Ellenőrizd a backend kapcsolatot.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ EXCEPTION in SaveAsync: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }


        partial void OnServiceIdChanged(int value)
        {
            Debug.WriteLine($"📝 ServiceId changed to: {value}");

            if (value > 0 && PartId == 0)
            {
                IsEdit = false;
                PageTitle = "Új alkatrész";
            }
        }


        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
