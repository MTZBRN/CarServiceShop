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

        [ObservableProperty]
        private bool isBusy;

        // Számított tulajdonság - Összesen ár
        public double TotalPrice => Quantity * Price;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public PartDetailViewModel(ApiService api)
        {
            _api = api;
            SaveCommand = new AsyncRelayCommand(SaveAsync, () => !IsBusy);
            CancelCommand = new AsyncRelayCommand(CancelAsync);
        }

        partial void OnPartIdChanged(int value)
        {
            if (value > 0)
            {
                IsEdit = true;
                PageTitle = "🔧 Alkatrész szerkesztése";
                _ = LoadAsync(value);
            }
            else
            {
                IsEdit = false;
                PageTitle = "➕ Új alkatrész";
            }
        }

        // Frissítjük a TotalPrice-t, amikor változik a quantity vagy price
        partial void OnQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        partial void OnPriceChanged(double value)
        {
            OnPropertyChanged(nameof(TotalPrice));
        }

        partial void OnIsBusyChanged(bool value)
        {
            SaveCommand.NotifyCanExecuteChanged();
        }

        private async Task LoadAsync(int id)
        {
            try
            {
                IsBusy = true;
                Debug.WriteLine($"🔍 Loading part with ID: {id}");
                
                var p = await _api.GetPartByIdAsync(id);
                if (p != null)
                {
                    PartNumber = p.PartNumber ?? string.Empty;
                    Name = p.Name ?? string.Empty;
                    Description = p.Description ?? string.Empty;
                    Quantity = p.Quantity;
                    Price = p.Price;
                    
                    Debug.WriteLine($"✅ Part loaded: {Name}");
                }
                else
                {
                    Debug.WriteLine($"❌ Part with ID {id} not found");
                    await Shell.Current.DisplayAlert("Hiba", "Az alkatrész nem található!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading part: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni az alkatrészt!", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                IsBusy = true;

                Debug.WriteLine($"🔍 ServiceId: {ServiceId}");
                Debug.WriteLine($"🔍 PartNumber: {PartNumber}");
                Debug.WriteLine($"🔍 Name: {Name}");

                // Részletes validáció
                if (string.IsNullOrWhiteSpace(PartNumber))
                {
                    await Shell.Current.DisplayAlert("Validációs hiba", "A cikkszám megadása kötelező!", "OK");
                    return;
                }
                
                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert("Validációs hiba", "Az alkatrész nevének megadása kötelező!", "OK");
                    return;
                }
                
                if (Quantity <= 0)
                {
                    await Shell.Current.DisplayAlert("Validációs hiba", "A mennyiség csak pozitív szám lehet!", "OK");
                    return;
                }
                
                if (Price < 0)
                {
                    await Shell.Current.DisplayAlert("Validációs hiba", "Az ár nem lehet negatív!", "OK");
                    return;
                }

                Debug.WriteLine($"💾 Saving part: {Name} (ServiceId: {ServiceId})");

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

                bool success;
                if (IsEdit)
                {
                    Debug.WriteLine($"🔄 Updating part ID: {PartId}");
                    success = await _api.UpdatePartAsync(part);
                }
                else
                {
                    Debug.WriteLine($"➕ Creating new part");
                    success = await _api.AddPartAsync(part);
                }

                if (success)
                {
                    Debug.WriteLine($"✅ Part saved successfully");
                    
                    // Értesítjük a PartListPage-t a változásról
                    WeakReferenceMessenger.Default.Send(new PartChangedMessage(ServiceId));

                    await Shell.Current.DisplayAlert("Siker", 
                        IsEdit ? "Az alkatrész sikeresen módosítva!" : "Az alkatrész sikeresen hozzáadva!", 
                        "OK");
                    
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    Debug.WriteLine($"❌ Failed to save part");
                    await Shell.Current.DisplayAlert("Hiba", 
                        IsEdit ? "Nem sikerült módosítani az alkatrészt!" : "Nem sikerült hozzáadni az alkatrészt!", 
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error saving part: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Váratlan hiba történt: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CancelAsync()
        {
            Debug.WriteLine($"❌ Part editing cancelled");
            await Shell.Current.GoToAsync("..");
        }
    }
}