using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using SmartEnergy.Api;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Database.Models;
using SmartEnergy.Extensions;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;

namespace SmartEnergy.ViewModels
{
    public partial class SettingsDeviceViewModel : BaseViewModel, IMessageReceiver
    {
        private bool _settingRelay = false;

        private bool _relay1;
        private bool _relay2;
        private bool _relay3;
        private bool _relay4;
        private readonly ILogService _logService;
        private readonly INavigationService _navigationService;
        private readonly UserService _userService;
        private readonly WebsocketClient _client;
        private readonly SmartEnergyApiService _apiService;

        [ObservableProperty]
        private bool _settingsMode = true;     
        
        [ObservableProperty]
        private string _logs = string.Empty;

        public SettingsDeviceViewModel(ILogService logService, INavigationService navigationService, UserService userService,
            WebsocketClient client, SmartEnergyApiService apiService)
        {
            _logService = logService;
            _navigationService = navigationService;
            _userService = userService;
            _client = client;
            _apiService = apiService;
        }

        public SceneDeviceItemViewModel Device { get; set; }
        public Action Deleted { get; set; }

        public bool Relay1
        {
            get => _relay1;
            set
            {
                if (SetProperty(ref _relay1, value))
                {
                    SetRelay(1);
                }
            }
        }   
        
        public bool Relay2
        {
            get => _relay2;
            set
            {
                if (SetProperty(ref _relay2, value))
                {
                    SetRelay(2);
                }
            }
        }       
        
        public bool Relay3
        {
            get => _relay3;
            set
            {
                if (SetProperty(ref _relay3, value))
                {
                    SetRelay(3);
                }
            }
        }  
        
        public bool Relay4
        {
            get => _relay4;
            set
            {
                if(SetProperty(ref _relay4, value))
                {
                    SetRelay(4);
                }
            }
        }

        public override Task Shown()
        {
            _client.Subscribe(this);
            return Task.CompletedTask;
        }

        public override ValueTask Disapear()
        {
            _client.Unsubscribe(this);
            return ValueTask.CompletedTask;
        }

        public async void OnMessage(string message)
        {
            if (await message.HandleExpiredTokenAsync(_navigationService, _userService, _client))
                return;

            var response = JsonConvert.DeserializeObject<DeviceResponseBase>(message);
            if (response == null)
                return;

            if(!response.IsState == true)
            { 
                if (response.Device.Equals(Device.Device.Mac))
                {
                    var data = JsonConvert.DeserializeObject<DeviceDataResponse>(message);

                    if (data != null)
                    {
                        if (_settingRelay)
                            return;

                        Logs += $"[{DateTime.Now:HH:mm:ss}] {message}\n\n";

                        Device.IsOnline = true;

                        _relay1 = data.Data.Relay1;
                        _relay2 = data.Data.Relay2;
                        _relay3 = data.Data.Relay3;
                        _relay4 = data.Data.Relay4;

                        OnPropertyChanged(nameof(Relay1));
                        OnPropertyChanged(nameof(Relay2));
                        OnPropertyChanged(nameof(Relay3));
                        OnPropertyChanged(nameof(Relay4));
                    }
                }
            }
        }

        [RelayCommand]
        public void ChangeMode(bool isSettingsMode)
        {
            SettingsMode = isSettingsMode;
        }   
        
        [RelayCommand]
        public async Task DeleteDeviceAsync()
        {
            var vm = await _navigationService.ShowPopupAsync<MessagePopupViewModel>(x =>
            {
                x.Message = $"Do you really want to delete device {Device.Device.Mac}?";
                x.IsConfirmation = true;
            });

            if (!vm.Cancelled)
            {
                Deleted?.Invoke();
                await _navigationService.GoBackAsync();
            }
        }

        private async void SetRelay(int relay)
        {
            if (!await CheckConnection(_navigationService))
                return;

            await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = $"Setting relay {relay}...");

            _settingRelay = true;

            ApiResult<SetRelayResponse> result = null;

            switch (relay)
            {
                case 1:
                    result = await _apiService.SetRelay1Async(Device.Device.Token, Relay1);
                    break;
                case 2:
                    result = await _apiService.SetRelay2Async(Device.Device.Token, Relay2);
                    break;
                case 3:
                    result = await _apiService.SetRelay3Async(Device.Device.Token, Relay3);
                    break;
                case 4:
                    result = await _apiService.SetRelay4Async(Device.Device.Token, Relay4);
                    break;
            }

            await Task.Delay(1000);

            await _navigationService.ClosePopupAsync();

            _settingRelay = false;
        }
    }
}
