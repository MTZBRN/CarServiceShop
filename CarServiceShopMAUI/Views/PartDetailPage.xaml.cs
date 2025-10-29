using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI.Views;

public partial class PartDetailPage : ContentPage
{
    public PartDetailPage(PartDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; 
    }
}