using CarServiceShopMAUI.ViewModels;
using System.Diagnostics;

namespace CarServiceShopMAUI.Views
{
    public partial class CarListPage : ContentPage
    {
        private readonly CarListPageViewModel _viewModel;

        // DI constructor
        public CarListPage(CarListPageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                Debug.WriteLine("📱 CarListPage appearing, loading cars...");
                await _viewModel.LoadCarsCommand.ExecuteAsync(null);
                Debug.WriteLine($"📱 Loaded {_viewModel.Cars.Count} cars");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error loading cars: {ex.Message}");
                await DisplayAlert("Hiba", $"Nem sikerült betölteni az autókat: {ex.Message}", "OK");
            }
        }
    }
}
