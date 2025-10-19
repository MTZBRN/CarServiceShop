using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI.Views
{
    public partial class CarDetailPage : ContentPage
    {
        public CarDetailPage(CarDetailViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
