using TaskManagement.Infrastructure.ViewModels;

namespace TaskManagementApp.Pages;

[QueryProperty(nameof(RoomId), ChatViewModel.ChatIdQueryKey)]
public partial class ChatPage : ContentPage
{

    private readonly ChatViewModel _viewModel;
    public ChatPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = MauiProgram.Services.GetService<ChatViewModel>();
    }

    public int RoomId { get; set; }

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
