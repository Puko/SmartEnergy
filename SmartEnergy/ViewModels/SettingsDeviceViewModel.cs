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
        
        [ObservableProperty]
        private string _relayName1;
        
        [ObservableProperty]
        private string _relayName2;

        [ObservableProperty]
        private string _relayName3;

        [ObservableProperty]
        private string _relayName4;

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

        public override async Task InitializeAsync()
        {
            var result = await _apiService.GetDeviceRelaySettings(Device.Device.Token);
            if(result.Succes)
            {
                RelayName1 = result.Value.Settings.R1.Name;
                RelayName2 = result.Value.Settings.R2.Name;
                RelayName3 = result.Value.Settings.R3.Name;
                RelayName4 = result.Value.Settings.R4.Name;
            }
            else
            {
                RelayName1 = "Relay 1";
                RelayName2 = "Relay 2";
                RelayName3 = "Relay 3";
                RelayName4 = "Relay 4";
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
                x.Message = string.Format(Localization["DeleteDeviceMessage"].ToString(), Device.Device.Mac);
                x.IsConfirmation = true;
            });

            if (!vm.Cancelled)
            {
                Deleted?.Invoke();
                await _navigationService.GoBackAsync();
            }
        }

        [RelayCommand]
        public async Task EditRelayNameAsync(string relay)
        {
            if (!await CheckConnection(_navigationService))
                return;

            _client.Unsubscribe(this);

            ScenePopupViewModel vm = new ScenePopupViewModel(_navigationService)
            {
                Name = GetRelayName(relay)
            };

            await _navigationService.ShowPopupAsync(vm);

            if (!vm.Cancelled)
            {
                await EditRelayAysnc(async () =>
                {
                    ApiResult<SetRelayResponse> result = null;
                    switch (relay)
                    {
                        case "1":
                            result = await _apiService.SetRelay1NameAsync(Device.Device.Token, vm.Name);
                           if(result.Succes)
                                RelayName1 = vm.Name;
                            break;
                        case"2":
                            result = await _apiService.SetRelay2NameAsync(Device.Device.Token, vm.Name);
                            if (result.Succes)
                                RelayName2 = vm.Name;
                            break;
                        case "3":
                            result = await _apiService.SetRelay3NameAsync(Device.Device.Token, vm.Name);
                            if (result.Succes)
                                RelayName3 = vm.Name;
                            break;
                        case "4":
                            result = await _apiService.SetRelay4NameAsync(Device.Device.Token, vm.Name);
                            if (result.Succes)
                                RelayName4 = vm.Name;
                            break;
                    }

                    return result;
                }, Localization["ChangingRelayName"].ToString());
            }

            _client.Subscribe(this);

        }

        private async void SetRelay(int relay)
        {
            _client.Unsubscribe(this);

            await EditRelayAysnc(async () =>
            {
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

                return result;
            }, Localization["SettingRelay"].ToString());

            _client.Subscribe(this);
        }

        private async Task EditRelayAysnc(Func<Task<ApiResult<SetRelayResponse>>> editRelay, string message)
        {
            if (!await CheckConnection(_navigationService))
                return;

            await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = message);

            _settingRelay = true;

            ApiResult<SetRelayResponse> result = await editRelay.Invoke();

            await Task.Delay(1000);

            await _navigationService.ClosePopupAsync();

            if (result?.Succes == true)
                _logService.Info($"Set relay status: {result.Value.Status}, message: {result.Value.Message}");
            else
            {
                var response = result.Response;
                if(response != null)
                    await _navigationService.ShowPopupAsync<MessagePopupViewModel>(x => x.Message = result.Response.Message);
                
                _logService.Warning($"Set relay status failed. Code: {result?.StatusCode}, message: {result?.Message}");
            }

            _settingRelay = false;
        }

        private string GetRelayName(string relay) => relay switch
        {
            "1" => RelayName1,
            "2" => RelayName2,
            "3" => RelayName3,
            "4" => RelayName4,
            _ => $"relay_{relay}",
        };
    }
}
