using Silence.Infrastructure.ViewModels;

namespace SilenceApp.Pages;

[QueryProperty(nameof(RoomId), ChatViewModel.ChatIdQueryKey)]
public partial class ChatPage : ContentPage
{

    private readonly ChatViewModel _viewModel;

    public int RoomId { get; set; }

    public ChatPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = MauiProgram.Services.GetService<ChatViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            await _viewModel.InitializeAsync(RoomId);
        }
        catch
        {
            // TODO: handle this
        }
    }
}
