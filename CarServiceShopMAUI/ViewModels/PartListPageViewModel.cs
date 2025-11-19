using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CarServiceShopMAUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(ServiceId), nameof(ServiceId))]
    public partial class PartListPageViewModel : ObservableObject
    {
        private readonly ApiService _api;

        [ObservableProperty]
        private int serviceId;

        [ObservableProperty]
        private string pageTitle = "Alkatrészek";

        [ObservableProperty]
        private ObservableCollection<Part> parts = new();

        [ObservableProperty]
        private Part selectedPart;

        public IAsyncRelayCommand LoadPartsCommand { get; }
        public IAsyncRelayCommand AddPartCommand { get; }
        public IAsyncRelayCommand BackCommand { get; }

        // Row-alapú parancsok
        public IAsyncRelayCommand<Part> EditPartRowCommand { get; }
        public IAsyncRelayCommand<Part> DeletePartRowCommand { get; }

        // Copy parancs
        public IAsyncRelayCommand<Part> CopyPartNumberCommand { get; }

        public PartListPageViewModel(ApiService api)
        {
            _api = api;

            LoadPartsCommand = new AsyncRelayCommand(LoadPartsAsync);
            AddPartCommand = new AsyncRelayCommand(AddPartAsync);
            BackCommand = new AsyncRelayCommand(BackAsync);

            EditPartRowCommand = new AsyncRelayCommand<Part>(EditPartRowAsync);
            DeletePartRowCommand = new AsyncRelayCommand<Part>(DeletePartRowAsync);

            // Copy command inicializálása
            CopyPartNumberCommand = new AsyncRelayCommand<Part>(CopyPartNumberAsync);

            WeakReferenceMessenger.Default.Register<PartChangedMessage>(this, async (r, m) =>
            {
                if (m.Value == ServiceId)
                {
                    await LoadPartsAsync();
                }
            });
        }

        partial void OnServiceIdChanged(int value)
        {
            if (value > 0)
            {
                PageTitle = $"Alkatrészek - Szerviz #{value}";
                _ = LoadPartsAsync();
            }
        }

        public async Task LoadPartsAsync()
        {
            try
            {
                Debug.WriteLine($"🔩 Loading parts for ServiceId={ServiceId}");
                var list = await _api.GetPartsForServiceAsync(ServiceId);
                Parts.Clear();
                foreach (var p in list)
                {
                    Parts.Add(p);
                }
                Debug.WriteLine($"✅ Loaded {Parts.Count} parts");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading parts: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni az alkatrészeket!", "OK");
            }
        }

        private async Task AddPartAsync()
        {
            try
            {
                Debug.WriteLine($"➕ Navigating to add part for ServiceId: {ServiceId}");
                var nav = new Dictionary<string, object>
                {
                    { "ServiceId", ServiceId }
                };
                await Shell.Current.GoToAsync(nameof(PartDetailPage), nav);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to add part: {ex.Message}");
            }
        }

        private async Task EditPartRowAsync(Part part)
        {
            if (part == null)
            {
                Debug.WriteLine("⚠️ EditPartRowAsync: part is null");
                return;
            }

            try
            {
                Debug.WriteLine($"✏️ Editing part: {part.Name} (ID: {part.Id})");
                var nav = new Dictionary<string, object>
                {
                    { "ServiceId", ServiceId },
                    { "PartId", part.Id }
                };
                await Shell.Current.GoToAsync(nameof(PartDetailPage), nav);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to edit part: {ex.Message}");
            }
        }

        private async Task DeletePartRowAsync(Part part)
        {
            if (part == null)
            {
                Debug.WriteLine("⚠️ DeletePartRowAsync: part is null");
                return;
            }

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törlöd az alkatrészt?\n\n{part.Name}\nCikkszám: {part.PartNumber}",
                "Igen",
                "Nem");

            if (!confirm) return;

            try
            {
                Debug.WriteLine($"🗑️ Deleting part: {part.Name} (ID: {part.Id})");
                bool success = await _api.DeletePartAsync(part.Id);

                if (success)
                {
                    Parts.Remove(part);
                    Debug.WriteLine("✅ Part deleted successfully");
                    await Shell.Current.DisplayAlert("Siker", "Alkatrész törölve!", "OK");
                }
                else
                {
                    Debug.WriteLine("❌ Delete failed - API returned false");
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült törölni az alkatrészt!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting part: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        // ✅ Cikkszám másolása a vágólapra (Toast nélkül, egyszerű verzió)
        private async Task CopyPartNumberAsync(Part part)
        {
            if (part == null || string.IsNullOrWhiteSpace(part.PartNumber))
            {
                Debug.WriteLine("⚠️ CopyPartNumberAsync: part or PartNumber is null/empty");
                return;
            }

            try
            {
                Debug.WriteLine($"📋 Copying part number: {part.PartNumber}");

                // Vágólapra másolás
                await Clipboard.Default.SetTextAsync(part.PartNumber);

                Debug.WriteLine("✅ Part number copied to clipboard");

                // Rövid visszajelzés (3 másodperc múlva eltűnik)
                var alert = Shell.Current.DisplayAlert(
                    "Másolva",
                    $"Cikkszám vágólapra másolva:\n{part.PartNumber}",
                    "OK");

                // 2 másodperc múlva automatikusan bezárjuk (nem vár rá a felhasználó)
                _ = Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    // Az alert automatikusan bezáródik amikor a felhasználó OK-ra kattint
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error copying part number: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült másolni a cikkszámot!", "OK");
            }
        }

        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    public sealed class PartChangedMessage : ValueChangedMessage<int>
    {
        public PartChangedMessage(int serviceId) : base(serviceId) { }
    }
}
