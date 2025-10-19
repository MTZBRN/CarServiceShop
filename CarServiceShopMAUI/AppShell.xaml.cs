using CarServiceShopMAUI.Views;

namespace CarServiceShopMAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CarListPage), typeof(CarListPage));
        Routing.RegisterRoute(nameof(ServicePage), typeof(ServicePage));
        Routing.RegisterRoute(nameof(InvoicePage), typeof(InvoicePage));


    }
    
}