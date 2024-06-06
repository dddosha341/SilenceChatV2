using SilenceApp.Pages;
using SilenceApp.Services;
using Silence.Infrastructure.Services;
using SilenceApp.ViewModels;

namespace SilenceApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();

        BindingContext = MauiProgram.Services.GetRequiredService<AppShellViewModel>();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(Route.Login.MapRouteToPath(), typeof(LoginPage));
        Routing.RegisterRoute(Route.Register.MapRouteToPath(), typeof(RegisterPage));
        Routing.RegisterRoute(Route.Welcome.MapRouteToPath(), typeof(WelcomePage));
        Routing.RegisterRoute(Route.ChatRoom.MapRouteToPath(), typeof(ChatPage));
       
    }
}