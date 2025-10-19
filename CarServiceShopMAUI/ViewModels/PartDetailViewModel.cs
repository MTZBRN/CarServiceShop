using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(ServiceId), nameof(ServiceId))]
    [QueryProperty(nameof(PartId), nameof(PartId))]
    public partial class PartDetailViewModel : ObservableObject
    {
        private readonly ApiService _api;

        [ObservableProperty]
        private int serviceId;

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
        private double price = 0;

        [ObservableProperty]
        private bool isEdit;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public PartDetailViewModel(ApiService api)
        {
            _api = api;
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        partial void OnPartIdChanged(int value)
        {
            if (value > 0)
            {
                IsEdit = true;
                PageTitle = "Alkatrész szerkesztése";
                _ = LoadAsync(value);
            }
            else
            {
                IsEdit = false;
                PageTitle = "Új alkatrész";
            }
        }

        private async Task LoadAsync(int id)
        {
            var p = await _api.GetPartByIdAsync(id);
            if (p != null)
            {
                PartNumber = p.PartNumber;
                Name = p.Name;
                Description = p.Description ?? string.Empty;
                Quantity = p.Quantity;
                Price = p.Price;
            }
        }

        private async Task SaveAsync()
        {
            // egyszerű validáció
            if (string.IsNullOrWhiteSpace(PartNumber))
            {
                await Shell.Current.DisplayAlert("Hiba", "A cikkszám kötelező!", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Hiba", "A név kötelező!", "OK");
                return;
            }
            if (Quantity <= 0)
            {
                await Shell.Current.DisplayAlert("Hiba", "A mennyiség legyen pozitív!", "OK");
                return;
            }
            if (Price < 0)
            {
                await Shell.Current.DisplayAlert("Hiba", "Az ár nem lehet negatív!", "OK");
                return;
            }

            var part = new Part
            {
                Id = PartId,
                ServiceId = ServiceId,
                PartNumber = PartNumber.Trim(),
                Name = Name.Trim(),
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
                Quantity = Quantity,
                Price = Price
            };

            bool ok;
            if (IsEdit)
                ok = await _api.UpdatePartAsync(part);
            else
                ok = await _api.AddPartAsync(part);

            if (ok)
            {
                // Szóljunk vissza a listának
                WeakReferenceMessenger.Default.Send(new PartChangedMessage(ServiceId));

                await Shell.Current.DisplayAlert("Siker",
                    IsEdit ? "Alkatrész módosítva!" : "Alkatrész hozzáadva!",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült menteni az alkatrészt!", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
