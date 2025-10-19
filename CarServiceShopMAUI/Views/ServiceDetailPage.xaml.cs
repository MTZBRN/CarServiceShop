using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI.Views
{
    public partial class ServiceDetailPage : ContentPage
    {
        public ServiceDetailPage(ServiceDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
