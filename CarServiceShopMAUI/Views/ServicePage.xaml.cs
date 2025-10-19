using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI.Views
{
    public partial class ServicePage : ContentPage
    {
        public ServicePage(ServicePageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = BindingContext as ServicePageViewModel;
            if (viewModel != null)
            {
                await viewModel.LoadServicesCommand.ExecuteAsync(null);
            }
        }
    }
}