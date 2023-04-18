using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Database.Models;
using SmartEnergy.Services;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Extensions;
using System.Collections.ObjectModel;
using SmartEnergy.Interfaces;
using Newtonsoft.Json;
using SmartEnergy.Api;

namespace SmartEnergy.ViewModels
{
    public partial class AddEditSceneViewModel : BaseViewModel, IMessageReceiver
    {
        private readonly INavigationService _navigationService;
        private readonly ILogService _logger;
        private readonly SceneDeviceService _sceneDeviceService;
        private readonly SceneService _sceneService;
        private readonly UserService _userService;
        private readonly WebsocketClient _websocketClient;
        private readonly SmartEnergyApiService _apiService;
        private MemoryStream _imageStream;
        private bool _editing;

        [ObservableProperty]
        private ImageSource _sceneImage;

        [ObservableProperty]
        private bool _graphicMode = true;

        [ObservableProperty]
        private Scene _scene = new Scene();

        [ObservableProperty]
        private ObservableCollection<AddSceneDeviceViewModel> _devices = new ObservableCollection<AddSceneDeviceViewModel>();

        public AddEditSceneViewModel(INavigationService navigationService, 
            ILogService logger, SceneDeviceService sceneDeviceService,
            SceneService sceneService, UserService userService, WebsocketClient websocketClient, 
            SmartEnergyApiService apiService)
        {
            _navigationService = navigationService;
            _logger = logger;
            _sceneDeviceService = sceneDeviceService;
            _sceneService = sceneService;
            _userService = userService;
            _websocketClient = websocketClient;
            _apiService = apiService;
        }

        public int? SceneId { get; set; }
        public Action Saved;

        public override async Task InitializeAsync()
        {
            if (SceneId.HasValue)
            {
                Scene = _sceneService.GetSceneById(SceneId.Value);
                Devices = new ObservableCollection<AddSceneDeviceViewModel>(Scene.Devices.Select(x => new AddSceneDeviceViewModel(x)));

                foreach (var item in Devices)
                {
                    await _websocketClient.SubscribeToMessagesAsync(true, true, item.Device.Token, _logger);
                }

                if(Scene.Image != null)
                    SceneImage = ImageSource.FromStream(() => new MemoryStream(Scene.Image));
            }
            else
            {
                await PickSceneImageAsync();
            }
        }

        public override Task Shown()
        {
            _websocketClient.Subscribe(this);
            return Task.CompletedTask;
        }

        public override ValueTask Disapear()
        {
            _websocketClient.Unsubscribe(this);
            return ValueTask.CompletedTask;
        }

        [RelayCommand]
        public async Task EditDeviceAsync(AddSceneDeviceViewModel sc)
        {
            var enabled = !sc.IsOnline;

            var result = await SetRelayAsync(sc.Device, enabled);
            if (result?.Succes == true)
            {
                sc.IsOnline = enabled;
            }
        }
        
        [RelayCommand]
        public async Task EditRelayAsync(AddSceneDeviceViewModel sc)
        {
            if (!await CheckConnection(_navigationService))
                return;

            _websocketClient.Unsubscribe(this);

            EntryViewModel vm = new EntryViewModel(_navigationService)
            {
                Name = sc.Device.Relay,
                Title = Localization["SceneName"].ToString()
            };

            await _navigationService.ShowPopupAsync(vm);

            if (!vm.Cancelled)
            {
                await EditRelayAsync(async () =>
                {
                    ApiResult<SetRelayResponse> result = null;
                    switch (sc.Device.RelayOrder)
                    {
                        case 1:
                            result = await _apiService.SetRelay1NameAsync(sc.Device.Token, vm.Name);
                            break;
                        case 2:
                            result = await _apiService.SetRelay2NameAsync(sc.Device.Token, vm.Name);
                            break;
                        case 3:
                            result = await _apiService.SetRelay3NameAsync(sc.Device.Token, vm.Name);
                            break;
                        case 4:
                            result = await _apiService.SetRelay4NameAsync(sc.Device.Token, vm.Name);
                            break;
                    }

                    if (result?.Succes == true)
                    {
                        sc.Name = vm.Name;
                        sc.Device.Relay = vm.Name;

                        _sceneDeviceService.Update(sc.Device);
                    }
                    
                    return result;
                }, Localization["ChangingRelayName"].ToString());
            }

            _websocketClient.Subscribe(this);
        }

        [RelayCommand]
        public async Task AddSceneDeviceAsync()
        {
            var datas = _userService.GetUserData().ToList();
           
            var viewModel = await _navigationService.ShowPopupAsync<SceneDeviceViewModel>(x => x.SetData(datas));

            if (!viewModel.Cancelled)
            {
                var existing = Devices.FirstOrDefault(x =>
                    x.Device.Mac == viewModel.Device.Mac && x.Device.Relay == viewModel.Relay &&
                    x.Device.RelayOrder == viewModel.RelayOrder);

                if (existing != null)
                {
                    await _navigationService.ShowPopupAsync<InfoViewModel>(x =>
                        x.Message = Localization["DeviceAlreadyInScene"].ToString());

                    return;
                }

                var sd = new SceneDevice
                {
                    Mac = viewModel.Device.Mac,
                    SceneId = Scene.Id,
                    Relay = viewModel.Relay,
                    RelayOrder = viewModel.RelayOrder,
                    Type = viewModel.Device.Type,
                    Token = viewModel.Device.Token
                };
                var vm = new AddSceneDeviceViewModel(sd);
                Devices.Add(vm);

                bool subscribed = await _websocketClient.SubscribeToMessagesAsync(false, true, sd.Token, _logger);
                if (!subscribed)
                {
                    Devices.Remove(vm);
                    await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = Localization["WebsocketConnectionfailed"].ToString());
                }
            }
        }

        [RelayCommand]
        public async Task SaveSceneAsync()
        {
            if (Scene.Id == 0)
            {
                EntryViewModel sc = new EntryViewModel(_navigationService)
                {
                    Name = Scene.Name,
                    Title = Localization["SceneName"].ToString()
                };

                await _navigationService.ShowPopupAsync(sc);
                if (sc.Cancelled)
                {
                    return;
                }

                Scene.Name = sc.Name;
            }

            if (Devices.Any())
            {
                Scene.Devices.Clear();
                foreach (var item in Devices)
                {
                    Scene.Devices.Add(item.Device);
                }
                
                if (Scene.Id == 0)
                    _sceneService.Add(Scene);
                else
                    _sceneService.Update(Scene);

                Saved?.Invoke();
                await _navigationService.GoBackAsync();
            }
        }
 
        [RelayCommand]
        public async Task DeleteDeviceAsync(AddSceneDeviceViewModel device)
        {
            var vm = await _navigationService.ShowPopupAsync<InfoViewModel>((Action<InfoViewModel>)(x =>
            {
                
                x.Message = string.Format(Localization["DeleteDeviceMessage"].ToString(), device.Device.Relay);
                x.IsConfirmation = true;
            }));

            if(!vm.Cancelled)
                Devices.Remove(device);
        }

        [RelayCommand]
        public async Task PickSceneImageAsync()
        {
            try
            {
                PickOptions options = PickOptions.Images;

                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        var stream = await result.OpenReadAsync();

                        using (MemoryStream ms = new MemoryStream())
                        {
                            await stream.CopyToAsync(ms);

                            Scene.Image = ms.ToArray();
                        }

                        if (_imageStream != null)
                        {
                            await _imageStream.DisposeAsync();
                            _imageStream = null;
                        }

                        _imageStream = new MemoryStream(Scene.Image);
                        SceneImage = ImageSource.FromStream(() => _imageStream);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Can't load image.");
                await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = Localization["ImageLoadingFailed"].ToString());
            }
        }

        [RelayCommand]
        public void ChangeMode()
        {
            GraphicMode = !GraphicMode;
        }

        public async Task<ApiResult<SetRelayResponse>> SetRelayAsync(SceneDevice device, bool enabled)
        {
            _websocketClient.Unsubscribe(this);

            var result = await EditRelayAsync(async () =>
            {
                ApiResult<SetRelayResponse> result = null;
                switch (device.RelayOrder)
                {
                    case 1:
                        result = await _apiService.SetRelay1Async(device.Token, enabled);
                        break;
                    case 2:
                        result = await _apiService.SetRelay2Async(device.Token, enabled);
                        break;
                    case 3:
                        result = await _apiService.SetRelay3Async(device.Token, enabled);
                        break;
                    case 4:
                        result = await _apiService.SetRelay4Async(device.Token, enabled);
                        break;
                }

                return result;
            },  enabled ? Localization["TurnOnRelay"].ToString() : Localization["TurnOffRelay"].ToString());

            _websocketClient.Subscribe(this);

            return result;
        }

        private async Task<ApiResult<SetRelayResponse>> EditRelayAsync(Func<Task<ApiResult<SetRelayResponse>>> editRelay, string message)
        {
            if (_editing)
                return null;

            _editing = true;

            if (!await CheckConnection(_navigationService))
                return null;

            await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = message);
            
            ApiResult<SetRelayResponse> result = await editRelay.Invoke();

            await Task.Delay(1000);

            await _navigationService.ClosePopupAsync();

            if (result?.Succes == true)
                _logger.Info($"Set relay status: {result.Value.Status}, message: {result.Value.Message}");
            else
            {
                var response = result?.Response;
                if (response != null)
                    await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = result.Response.Message);

                _logger.Warning($"Set relay status failed. Code: {result?.StatusCode}, message: {result?.Message}");
            }


            _editing = false;

            return result;
        }

        public async void OnMessage(string message)
        {
            if (await message.HandleExpiredTokenAsync(_navigationService, _userService, _websocketClient))
                return;

            var deviceResponse = JsonConvert.DeserializeObject<DeviceResponseBase>(message);
            if (deviceResponse != null)
            {
                var device = Devices.FirstOrDefault(x => x.Device.Mac.Equals(deviceResponse.Device));
                if (device != null)
                {
                    if (deviceResponse.IsState == false)
                    {
                        var data = JsonConvert.DeserializeObject<DeviceDataResponse>(message);

                        if (data != null)
                        {
                            var isEnabled = data.Data.GetRelayState(device.Device.RelayOrder);
                            device.IsOnline = isEnabled;
                        }
                    }
                }
            }
        }
    }
}
