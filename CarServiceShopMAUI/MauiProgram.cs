using Microsoft.Extensions.Logging;
using CarServiceShopMAUI.Services;
using CarServiceShopMAUI.Views;
using CarServiceShopMAUI.ViewModels;

namespace CarServiceShopMAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register HttpClient with base address
        builder.Services.AddHttpClient<ApiService>(client =>
        {
            // FIX: Használd HTTP-t az 5083-as porton
            client.BaseAddress = new Uri("http://localhost:5083/api/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register ViewModels
        builder.Services.AddTransient<CarListPageViewModel>();

        // Register Pages
        builder.Services.AddTransient<CarListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
