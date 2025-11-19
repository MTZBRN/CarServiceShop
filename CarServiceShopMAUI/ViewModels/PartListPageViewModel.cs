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
        public IAsyncRelayCommand EditPartCommand { get; }
        public IAsyncRelayCommand DeletePartCommand { get; }
        public IAsyncRelayCommand BackCommand { get; }

        public PartListPageViewModel(ApiService api)
        {
            _api = api;

            LoadPartsCommand = new AsyncRelayCommand(LoadPartsAsync);
            AddPartCommand = new AsyncRelayCommand(AddPartAsync);
            EditPartCommand = new AsyncRelayCommand(EditPartAsync, CanModify);
            DeletePartCommand = new AsyncRelayCommand(DeletePartAsync, CanModify);
            BackCommand = new AsyncRelayCommand(BackAsync);

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

        partial void OnSelectedPartChanged(Part value)
        {
            EditPartCommand.NotifyCanExecuteChanged();
            DeletePartCommand.NotifyCanExecuteChanged();
        }

        private bool CanModify() => SelectedPart != null;

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
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading parts: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni az alkatrészeket!", "OK");
            }
        }

        private async Task AddPartAsync()
        {
            Debug.WriteLine($"🔍 Navigating with ServiceId: {ServiceId}");
            Debug.WriteLine($"AddPartAsync - aktuális ServiceId: {ServiceId}");


            var nav = new Dictionary<string, object>
            {
                { "ServiceId", ServiceId }
            };
            await Shell.Current.GoToAsync(nameof(PartDetailPage), nav);
        }

        private async Task EditPartAsync()
        {
            if (SelectedPart == null) return;
            var nav = new Dictionary<string, object>
            {
                { "ServiceId", ServiceId },
                { "PartId", SelectedPart.Id }
            };
            await Shell.Current.GoToAsync(nameof(PartDetailPage), nav);
        }

        private async Task DeletePartAsync()
        {
            if (SelectedPart == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törlöd az alkatrészt?\n{SelectedPart.Name} ({SelectedPart.PartNumber})",
                "Igen", "Nem");

            if (!confirm) return;

            try
            {
                bool ok = await _api.DeletePartAsync(SelectedPart.Id);
                if (ok)
                {
                    Parts.Remove(SelectedPart);
                    SelectedPart = null;
                    await Shell.Current.DisplayAlert("Siker", "Alkatrész törölve!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült törölni!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting part: {ex.Message}");
            }
        }

        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    public sealed class PartChangedMessage : ValueChangedMessage<int>
    {
        // value = ServiceId
        public PartChangedMessage(int serviceId) : base(serviceId) { }
    }
}
