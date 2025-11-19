using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI.Views;

public partial class PartListPage : ContentPage
{
    public PartListPage(PartListPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}