﻿using Microsoft.Extensions.Logging;
using Silence.Infrastructure.ViewModels;
using SilenceApp.Services;
using Silence.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using SilenceApp.ViewModels;

namespace SilenceApp;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .RegisterServices()
            .RegisterViewModels();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        Services = app.Services;

        return app;
    }

    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services
            .AddSingleton<INavigationService, NavigationService>()
            .AddSingleton<ISecureStorageService, SecureStorageService>()
            .AddSingleton<IAuthenticationService, Silence.Infrastructure.Services.AuthenticationService>()
            .AddSingleton<RefreshTokenHandler>()
            .AddSingleton<ApiClientService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ApiClientService>>();
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

#if WINDOWS
                return new(logger, httpClientFactory, "http://127.0.0.1:7071"); 
#elif ANDROID
                return new(logger, httpClientFactory, "http://10.0.2.2:7071"); // specify for real android phone manually
#endif
            })
            .AddHttpClient();

        builder.Services
            .AddHttpClient(ApiClientService.AutorizedHttpClient)
                .AddHttpMessageHandler<RefreshTokenHandler>();

        return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services
            .AddTransient<AppShellViewModel>()
            .AddTransient<LoginViewModel>()
            .AddTransient<RegisterViewModel>()
            .AddTransient<WelcomeViewModel>()
            .AddTransient<ChatViewModel>();

        return builder;
    }
}

