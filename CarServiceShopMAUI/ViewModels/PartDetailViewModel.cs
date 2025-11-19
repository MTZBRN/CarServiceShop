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
                    PartNumber = PartNumber.Trim(),
                    Name = Name.Trim(),
                    Description = Description.Trim(),
                    Quantity = Quantity,
                    NetPrice = NetPrice,
                    VATRate = VatRate,
                    ServiceId = ServiceId,
                    Service = new Service {Id = ServiceId }

                };

                bool success = IsEdit
                    ? await _apiService.UpdatePartAsync(part)
                    : await _apiService.AddPartAsync(part);

                if (success)
                {
                    await Shell.Current.DisplayAlert("Siker",
                        IsEdit ? "Alkatrész módosítva!" : "Alkatrész hozzáadva!",
                        "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Mentési hiba: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
