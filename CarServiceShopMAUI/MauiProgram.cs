﻿using Microsoft.Extensions.Logging;
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

        // ViewModels
        builder.Services.AddTransient<CarListPageViewModel>();
        builder.Services.AddTransient<CarDetailViewModel>();
        builder.Services.AddTransient<ServicePageViewModel>();
        builder.Services.AddTransient<ServiceDetailViewModel>();
        builder.Services.AddTransient<PartListPageViewModel>();
        builder.Services.AddTransient<PartDetailViewModel>();

        // Pages
        builder.Services.AddTransient<CarListPage>();
        builder.Services.AddTransient<CarDetailPage>();
        builder.Services.AddTransient<ServicePage>();
        builder.Services.AddTransient<ServiceDetailPage>();
        builder.Services.AddTransient<PartListPage>();
        builder.Services.AddTransient<PartDetailPage>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
