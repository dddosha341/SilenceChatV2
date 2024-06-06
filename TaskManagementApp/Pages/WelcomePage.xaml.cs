using Microsoft.Maui.Controls;
using TaskManagement.Infrastructure.ViewModels;

namespace TaskManagementApp.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _viewModel;

    public WelcomePage()
    {
        InitializeComponent();
        BindingContext = _viewModel = MauiProgram.Services.GetRequiredService<WelcomeViewModel>();

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.InitializeAsync();
        }
        catch
        {
            // TODO: handle error here - suggest to update page
        }
    }
}
