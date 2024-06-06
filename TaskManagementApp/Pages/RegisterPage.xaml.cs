using TaskManagement.Infrastructure.ViewModels;

namespace TaskManagementApp.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.Services.GetService<RegisterViewModel>();
    }
}
