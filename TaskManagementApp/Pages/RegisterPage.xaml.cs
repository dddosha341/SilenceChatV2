using Silence.Infrastructure.ViewModels;

namespace SilenceApp.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<RegisterViewModel>();
    }
}
