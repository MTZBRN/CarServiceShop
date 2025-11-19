using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CarServiceShopMAUI.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(CarId), nameof(CarId))]
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

        // Új: row-parancsok minden gombhoz
        public IAsyncRelayCommand<Service> EditServiceCommand { get; }
        public IAsyncRelayCommand<Service> DeleteServiceCommand { get; }
        public IAsyncRelayCommand<Service> ViewPartsCommand { get; }
        public IAsyncRelayCommand<Service> ExportWorksheetCommand { get; }
        public IAsyncRelayCommand AddServiceCommand { get; }
        public IAsyncRelayCommand BackCommand { get; }
        public IAsyncRelayCommand LoadServicesCommand { get; }

        public ServicePageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            LoadServicesCommand = new AsyncRelayCommand(LoadServicesAsync);
            AddServiceCommand = new AsyncRelayCommand(AddServiceAsync);
            EditServiceCommand = new AsyncRelayCommand<Service>(EditServiceAsync);
            DeleteServiceCommand = new AsyncRelayCommand<Service>(DeleteServiceAsync);
            ViewPartsCommand = new AsyncRelayCommand<Service>(ViewPartsAsync);
            ExportWorksheetCommand = new AsyncRelayCommand<Service>(ExportWorksheetAsync);
            BackCommand = new AsyncRelayCommand(BackAsync);
        }

        partial void OnCarIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadCarInfoAsync(value);
                _ = LoadServicesAsync();
            }
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

        private async Task EditServiceAsync(Service service)
        {
            if (service == null) return;

            try
            {
                var navParams = new Dictionary<string, object>
                {
                    { "ServiceId", service.Id },
                    { "CarId", CarId }
                };
                await Shell.Current.GoToAsync(nameof(ServiceDetailPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to edit service: {ex.Message}");
            }
        }

        private async Task DeleteServiceAsync(Service service)
        {
            if (service == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Megerősítés",
                $"Biztosan törlöd ezt a szervizt?\n{service.ServiceDescription}",
                "Igen",
                "Nem");

            if (!confirm) return;

            try
            {
                bool success = await _apiService.DeleteServiceAsync(service.Id);

                if (success)
                {
                    Services.Remove(service);
                    ServiceCount = Services.Count;
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

        private async Task ExportWorksheetAsync(Service service)
        {
            if (service == null) return;
            try
            {
                var navParams = new Dictionary<string, object>
                {
                    { "ServiceId", service.Id }
                };
                await Shell.Current.GoToAsync(nameof(WorksheetPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error navigating to worksheet: {ex.Message}");
            }
        }

        private async Task BackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
