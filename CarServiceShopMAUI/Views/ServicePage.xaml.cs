using CarServiceShopMAUI.Models;
using CarServiceShopMAUI.ViewModels;
using System.Diagnostics;

namespace CarServiceShopMAUI.Views
{
    public partial class ServicePage : ContentPage
    {
        public ServicePage(ServicePageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnWorksheetButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;

                // A gomb szülő Frame-jéből szerezzük meg a Service objektumot
                var frame = button?.Parent?.Parent as Frame;
                var service = frame?.BindingContext as Service;

                // Vagy próbáld ezt, ha a fenti nem működik:
                // var service = ((button?.Parent as StackLayout)?.Parent as Frame)?.BindingContext as Service;

                var viewModel = this.BindingContext as ServicePageViewModel;

                if (service == null)
                {
                    Debug.WriteLine("❌ Service is null! Trying alternative method...");

                    // Alternatív módszer: keresés a vizuális fában
                    var currentElement = button?.Parent;
                    while (currentElement != null && service == null)
                    {
                        service = currentElement.BindingContext as Service;
                        currentElement = currentElement.Parent;
                    }
                }

                if (service == null || viewModel == null)
                {
                    Debug.WriteLine($"❌ Service: {service != null}, ViewModel: {viewModel != null}");
                    await DisplayAlert("Hiba", "Nem sikerült azonosítani a szervizt!", "OK");
                    return;
                }

                Debug.WriteLine($"📄 Navigating - Service ID: {service.Id}, CarId: {viewModel.CarId}");

                var navParams = new Dictionary<string, object>
                {
                    { "CarId", viewModel.CarId },
                    { "ServiceId", service.Id }
                };

                await Shell.Current.GoToAsync(nameof(WorksheetPage), navParams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Navigation error: {ex.Message}\n{ex.StackTrace}");
                await DisplayAlert("Hiba", $"Hiba történt: {ex.Message}", "OK");
            }
        }
    }
}
