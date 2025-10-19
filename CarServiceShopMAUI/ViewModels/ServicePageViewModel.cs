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
    // Navigációs paraméter: CarId-t a hívó oldal adja át
    [QueryProperty(nameof(CarId), nameof(CarId))]
    [QueryProperty(nameof(Refresh), nameof(Refresh))]
    public partial class ServicePageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private int carId;

        [ObservableProperty]
        private string pageTitle = "Szervizek";

        [ObservableProperty]
        private string carInfo = string.Empty;

        [ObservableProperty]
        private int serviceCount;

        [ObservableProperty]
        private ObservableCollection<Service> services = new();

        [ObservableProperty]
        private Service selectedService;

        // Opcionális: ha navigációs paraméterből kapunk jelzést frissítésre
        [ObservableProperty]
        private bool refresh;

        public IAsyncRelayCommand LoadServicesCommand { get; }
        public IAsyncRelayCommand AddServiceCommand { get; }
        public IAsyncRelayCommand EditServiceCommand { get; }
        public IAsyncRelayCommand DeleteServiceCommand { get; }
        public IAsyncRelayCommand<Service> ViewPartsCommand { get; }
        public IAsyncRelayCommand BackCommand { get; }

        public ServicePageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            LoadServicesCommand = new AsyncRelayCommand(LoadServicesAsync);
            AddServiceCommand = new AsyncRelayCommand(AddServiceAsync);
            EditServiceCommand = new AsyncRelayCommand(EditServiceAsync, CanModifyService);
            DeleteServiceCommand = new AsyncRelayCommand(DeleteServiceAsync, CanModifyService);
            ViewPartsCommand = new AsyncRelayCommand<Service>(ViewPartsAsync);
            BackCommand = new AsyncRelayCommand(BackAsync);

            // Ha Service hozzáadás/szerkesztés után üzenetet kapunk, újratöltünk
            WeakReferenceMessenger.Default.Register<ServiceChangedMessage>(this, async (r, m) =>
            {
                if (m.Value == CarId)
                {
                    Debug.WriteLine("🔁 ServiceChangedMessage received -> reloading services");
                    await LoadServicesAsync();
                }
            });
        }

        // Ha a CarId megváltozik (navigáció), betöltjük az autó infót és a listát
        partial void OnCarIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadCarInfoAsync(value);
                _ = LoadServicesAsync();
            }
        }

        // Ha a SelectedService változik, a gombok engedélyezése frissüljön
        partial void OnSelectedServiceChanged(Service value)
        {
            EditServiceCommand.NotifyCanExecuteChanged();
            DeleteServiceCommand.NotifyCanExecuteChanged();
        }

        // Opcionális: ha navigációs paraméterből kérünk frissítést
        partial void OnRefreshChanged(bool value)
        {
            if (value)
            {
                _ = LoadServicesAsync();
            }
        }

        private bool CanModifyService()
        {
            return SelectedService != null;
        }

        private async Task LoadCarInfoAsync(int carId)
        {
            try
            {
                var car = await _apiService.GetCarByIdAsync(carId);
                if (car != null)
                {
                    CarInfo = $"{car.Brand} {car.Model} ({car.LicensePlate})";
                    PageTitle = $"Szervizek - {car.LicensePlate}";
                }
                else
                {
                    CarInfo = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading car info: {ex.Message}");
            }
        }

        public async Task LoadServicesAsync()
        {
            try
            {
                Debug.WriteLine($"🔄 Loading services for CarId={CarId}...");
                var servicesFromApi = await _apiService.GetServicesForCarAsync(CarId);

                Services.Clear();
                foreach (var s in servicesFromApi)
                {
                    // Ha szeretnél TotalCost-ot, itt számold
                    s.TotalPrice = s.WorkHours * s.WorkHourPrice;
                    Services.Add(s);
                }

                ServiceCount = Services.Count;
                Debug.WriteLine($"✅ Loaded {ServiceCount} services");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading services: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", "Nem sikerült betölteni a szervizeket!", "OK");
            }
        }

        private async Task AddServiceAsync()
        {
            try
            {
                Debug.WriteLine($"➕ Navigating to add service for CarId: {CarId}");
                var navParams = new Dictionary<string, object>
                {
                    { "CarId", CarId }
                };
                await Shell.Current.GoToAsync(nameof(ServiceDetailPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to add service: {ex.Message}");
            }
        }

        private async Task EditServiceAsync()
        {
            if (SelectedService == null) return;

            try
            {
                Debug.WriteLine($"✏️ Navigating to edit service ID: {SelectedService.Id}");
                var navParams = new Dictionary<string, object>
                {
                    { "ServiceId", SelectedService.Id },
                    { "CarId", CarId }
                };
                await Shell.Current.GoToAsync(nameof(ServiceDetailPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to edit service: {ex.Message}");
            }
        }

        private async Task DeleteServiceAsync()
        {
            if (SelectedService == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törlöd ezt a szervizt?\n{SelectedService.ServiceDescription}",
                "Igen",
                "Nem");

            if (!confirm) return;

            try
            {
                Debug.WriteLine($"🗑️ Deleting service ID: {SelectedService.Id}");
                bool success = await _apiService.DeleteServiceAsync(SelectedService.Id);

                if (success)
                {
                    Services.Remove(SelectedService);
                    ServiceCount = Services.Count;
                    SelectedService = null;
                    await Shell.Current.DisplayAlert("Siker", "Szerviz törölve!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Hiba", "Nem sikerült törölni a szervizt!", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error deleting service: {ex.Message}");
                await Shell.Current.DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }

        private async Task ViewPartsAsync(Service service)
        {
            if (service == null) return;

            try
            {
                Debug.WriteLine($"🔧 Navigating to parts for service ID: {service.Id}");
                var navParams = new Dictionary<string, object>
                {
                    { "ServiceId", service.Id }
                };
                await Shell.Current.GoToAsync($"{nameof(PartListPage)}", navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to parts: {ex.Message}");
            }
        }

        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    // Messenger üzenet, ha szerviz változott (hozzáadás/szerkesztés/törlés)
    public sealed class ServiceChangedMessage : ValueChangedMessage<int>
    {
        public ServiceChangedMessage(int carId) : base(carId) { }
    }
}
