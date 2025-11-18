using CarServiceShopMAUI.Views;

namespace CarServiceShopMAUI;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(CarListPage), typeof(CarListPage));
        Routing.RegisterRoute(nameof(ServicePage), typeof(ServicePage));
        Routing.RegisterRoute(nameof(CarDetailPage), typeof(CarDetailPage));
        Routing.RegisterRoute(nameof(ServiceDetailPage), typeof(ServiceDetailPage));
        Routing.RegisterRoute(nameof(PartListPage), typeof(PartListPage));
        Routing.RegisterRoute(nameof(PartDetailPage), typeof(PartDetailPage));
        Routing.RegisterRoute(nameof(WorksheetPage), typeof(WorksheetPage));



    }

}