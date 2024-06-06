using TaskManagement.Infrastructure.ViewModels;

namespace TaskManagementApp.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<LoginViewModel>();
    }
}
