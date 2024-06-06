using Silence.Infrastructure.ViewModels;

namespace SilenceApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<LoginViewModel>();
    }
}
