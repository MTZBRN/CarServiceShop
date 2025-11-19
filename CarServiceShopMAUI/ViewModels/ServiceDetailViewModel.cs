using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace CarServiceShopMAUI.ViewModels
{
    [QueryProperty(nameof(ServiceId), nameof(ServiceId))]
    [QueryProperty(nameof(CarId), nameof(CarId))]
    public partial class ServiceDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private int serviceId;

        [ObservableProperty]
        private int carId;

        [ObservableProperty]
        private string pageTitle = "Új szerviz";

        [ObservableProperty]
        private string serviceDescription = string.Empty;

        [ObservableProperty]
        private double workHours = 1;

        [ObservableProperty]
        private double workHourPrice = 15000;

        [ObservableProperty]
        private DateTime serviceDate = DateTime.Now;

        [ObservableProperty]
        private double estimatedCost;

        [ObservableProperty]
        private bool isEdit;

        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand CancelCommand { get; }

        public ServiceDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new AsyncRelayCommand(CancelAsync);

            CalculateEstimatedCost();
            Debug.WriteLine($"🏗️ ServiceDetailViewModel created");
        }

        // ✅ KRITIKUS: ServiceId alapján állítsd be az IsEdit-et!
        partial void OnServiceIdChanged(int value)
        {
            Debug.WriteLine($"📝 ServiceId changed to: {value}");

            if (value > 0)
            {
                // ✅ Van ID → Szerkesztés
                IsEdit = true;
                PageTitle = "Szerviz szerkesztése";
                _ = LoadServiceAsync(value);
            }
            else
            {
                // ✅ Nincs ID (0) → Új szerviz
                IsEdit = false;
                PageTitle = "Új szerviz";
            }
        }

        // ✅ CarId változásakor NE állítsd át az IsEdit-et!
        partial void OnCarIdChanged(int value)
        {
            Debug.WriteLine($"📝 CarId changed to: {value}");

            // Ha új szerviz (ServiceId == 0), akkor maradjon IsEdit = false
            if (ServiceId == 0)
            {
                IsEdit = false;
                PageTitle = "Új szerviz";
            }
        }

        partial void OnWorkHoursChanged(double value)
        {
            CalculateEstimatedCost();
        }

        partial void OnWorkHourPriceChanged(double value)
        {
            CalculateEstimatedCost();
        }

        private void CalculateEstimatedCost()
        {
            EstimatedCost = (double)(WorkHours * WorkHourPrice);
        }

        private async Task LoadServiceAsync(int id)
        {
            try
            {
                Debug.WriteLine($"🔄 Loading service with ID: {id}");
                var service = await _apiService.GetServiceByIdAsync(id);

                if (service != null)
                {
                    ServiceDescription = service.ServiceDescription;
                    WorkHours = service.WorkHours;
                    WorkHourPrice = service.WorkHourPrice;
                    ServiceDate = service.ServiceDate;
                    CarId = service.CarId;

                    Debug.WriteLine($"✅ Service loaded: ID={service.Id}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading service: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                Debug.WriteLine($"💾 Save started - CarId: {CarId}, ServiceId: {ServiceId}, IsEdit: {IsEdit}");

                if (CarId == 0)
                {
                    Debug.WriteLine("❌ ERROR: CarId is 0!");
                    await Shell.Current.DisplayAlert("Hiba", "Hiányzik az autó azonosító!", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(ServiceDescription))
                {
                    await Shell.Current.DisplayAlert("Hiba", "A leírás megadása kötelező!", "OK");
                    return;
                }

                if (WorkHours <= 0)
                {
                    await Shell.Current.DisplayAlert("Hiba", "A munkaóra nem lehet 0 vagy negatív!", "OK");
                    return;
                }

                if (WorkHourPrice <= 0)
                {
                    await Shell.Current.DisplayAlert("Hiba", "Az óradíj nem lehet 0 vagy negatív!", "OK");
                    return;
                }

                var service = new Service
                {
                    Id = ServiceId,  // ✅ Ha új: 0, ha szerkesztés: meglévő ID
                    CarId = CarId,
                    ServiceDescription = ServiceDescription.Trim(),
                    WorkHours = WorkHours,
                    WorkHourPrice = WorkHourPrice,
                    ServiceDate = ServiceDate
                };

                Debug.WriteLine($"📦 Service object: Id={service.Id}, CarId={service.CarId}, IsEdit={IsEdit}");

                bool success;

                // ✅ KRITIKUS: IsEdit ÉS ServiceId > 0 alapján!
                if (IsEdit && ServiceId > 0)
                {
                    Debug.WriteLine($"✏️ UPDATING service ID: {service.Id}");
                    success = await _apiService.UpdateServiceAsync(service);
                }
                else
                {
                    Debug.WriteLine($"➕ ADDING new service to Car ID: {service.CarId}");
                    success = await _apiService.AddServiceAsync(service);
                }

                Debug.WriteLine($"🔍 API result: {success}");

                if (success)
                {
                    Debug.WriteLine("✅ Save successful!");
                    await Shell.Current.DisplayAlert("Siker",
                        IsEdit ? "Szerviz módosítva!" : "Szerviz hozzáadva!",
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

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}