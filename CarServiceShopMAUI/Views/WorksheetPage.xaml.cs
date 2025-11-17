using CarServiceShopMAUI.ViewModels;
namespace CarServiceShopMAUI.Views;

public partial class WorksheetPage : ContentPage
{
	public WorksheetPage(WorksheetViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}