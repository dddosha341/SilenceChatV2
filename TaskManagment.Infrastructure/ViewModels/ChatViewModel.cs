using Microsoft.AspNetCore.SignalR.Client;
using TaskManagement.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructure.ViewModels;
using System.Windows.Input;
using System.Data;
using TaskManagement.Infrastructure.Utils;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Infrastructure.DataContracts;
using System.Net;

namespace TaskManagement.Infrastructure.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public const string ChatIdQueryKey = "chatId";

        private readonly HubConnection _hubConnection;

        private readonly IAuthenticationService _authenticationService;

        private readonly INavigationService _navigationService;

        private readonly ApiClientService _apiClientService;

        private readonly ISecureStorageService _secureStorageService;

        public IEnumerable<MessageViewModel> Messages { get; private set; }

        public ChatViewModel(IAuthenticationService authenticationService, 
            ApiClientService apiClientService,
            INavigationService navigationService,
            ISecureStorageService secureStorageService)
        {
            //_hubConnection = new HubConnectionBuilder()
             //   .WithUrl("http://localhost:7071/chatHub")
              //  .Build();
            

            _authenticationService = authenticationService;
            _apiClientService = apiClientService;
            _navigationService = navigationService;
            _secureStorageService = secureStorageService;

            DeleteRoomCommand = new RelayCommand(DeleteRoom);

            EditRoomCommand = new RelayCommand(EditRoom);

            SendMessageCommand = new RelayCommand(SendMessage);

            ApplyCommand = new RelayCommand(Apply);

        }

        private RoomViewModel _room;

        public RoomViewModel Room { get { return _room; } private set { _room = value; } }

        private bool _isEditing = false;

        private string _newRoomName = string.Empty;

        public string NewRoomName
        {
            get { return _newRoomName; } 
            set 
            {
                if (!value.IsNullOrEmpty())
                    _newRoomName = value;
            }
        }

        public string NewMessage
        {
            get; set;
        }


        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                }
            }
        }
        public async Task InitializeAsync(int roomId,
                CancellationToken cancellationToken = default)
        {
            var roomResponse = await _apiClientService
               .GetRoomAsync(roomId, cancellationToken);

            Update(roomResponse);


            Messages = await _apiClientService
                .GetMessagesAsync(this.Room.Name, cancellationToken);
            OnPropertyChanged(nameof(Messages));

            _hubConnection.On<MessageViewModel>("newMessage,", FormatMessage);



            //await _hubConnection.StartAsync();

            //await _hubConnection.SendAsync("Join", this._room.Name);




            
        }


        public ICommand DeleteRoomCommand { get; private set; }

        // Команда для редактирования комнаты
        public ICommand EditRoomCommand { get; private set; }

        // Команда для применения изменений
        public ICommand ApplyCommand { get; private set; }

        public ICommand SendMessageCommand { get; private set; }

        private async Task SendMessage()
        {
            var admin = await _secureStorageService.GetAsync(SecureStorageKey.Username);
            MessageViewModel viewModel = new MessageViewModel()
            {
                Avatar = "false",
                Content = NewMessage,
                FromFullName = admin,
                Timestamp = DateTime.Now,
                FromUserName = admin,
                Room = this.Room.Name,
                
            };

            var response = await _apiClientService.SendMessage(viewModel);

            if(response is not null)
            {
                if(response .StatusCode == HttpStatusCode.OK) 
                {
                    NewMessage = string.Empty;
                    OnPropertyChanged(nameof(NewMessage));
                }
            }
        }

        private void Update(RoomViewModel roomViewModel)
        {
            Room = roomViewModel;

            OnPropertyChanged(nameof(Room));
        }

        private async void DeleteRoom()
        {
            //await _hubConnection.SendAsync("Leave", _room.Name);

           var response = await _apiClientService.DeleteRoomAsync(Room.Id);

            if (response != null) 
            {
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    await _navigationService.GoToAsync(Route.Welcome, keepHistory: false);
                }
            }
        }

        private async Task FormatMessage(MessageViewModel message)
        {
            Messages.Append(message);
            OnPropertyChanged(nameof(Messages));
        }

        private void EditRoom()
        {
            
            if (!IsEditing)
                IsEditing = true;
            else
                IsEditing = false;
            // Обработка нажатия кнопки Edit Room
            OnPropertyChanged(nameof(IsEditing));
        }

        private async void Apply()
        {
            if(NewRoomName is not null)
            {
                var newRoomModel = new RoomViewModel()
                {
                    Id = this.Room.Id,
                    Admin = this.Room.Admin,
                    Name = NewRoomName
                };
                var response = await _apiClientService.EditRoomAsync(
                    this.Room.Id,
                    newRoomModel
                    );
                if (response.IsSuccessStatusCode)
                {
                    Update(newRoomModel);
                }
            }


            // Обработка нажатия кнопки Apply
            // Выполнение команды
            IsEditing = false; // Сброс свойства IsEditing в false после нажатия кнопки Apply
            OnPropertyChanged(nameof(IsEditing));
        }


    }
}
