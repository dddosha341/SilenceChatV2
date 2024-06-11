using Silence.Infrastructure.ViewModels;

namespace SilenceApp.Pages;

public partial class EditProfilePage : ContentPage
{
    private readonly EditProfileViewModel _viewModel;

	public EditProfilePage()
	{
		InitializeComponent();
        BindingContext = _viewModel = MauiProgram.Services.GetService<EditProfileViewModel>();
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