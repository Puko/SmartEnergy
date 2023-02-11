using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Database.Models;
using SmartEnergy.Services;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Extensions;
using System.Collections.ObjectModel;
using SmartEnergy.Interfaces;
using Newtonsoft.Json;

namespace SmartEnergy.ViewModels
{
    public partial class AddEditSceneViewModel : BaseViewModel, IMessageReceiver
    {
        private readonly INavigationService _navigationService;
        private readonly ILogService _logger;
        private readonly SceneService _sceneService;
        private readonly UserService _userService;
        private readonly WebsocketClient _websocketClient;

        [ObservableProperty]
        private ImageSource _sceneImage;

        [ObservableProperty]
        private bool _graphicMode = true;

        [ObservableProperty]
        private Scene _scene = new Scene();

        [ObservableProperty]
        private ObservableCollection<SceneDeviceItemViewModel> _devices = new ObservableCollection<SceneDeviceItemViewModel>();

        public AddEditSceneViewModel(INavigationService navigationService, ILogService logger,
            SceneService sceneService, UserService userService, WebsocketClient websocketClient)
        {
            _navigationService = navigationService;
            _logger = logger;
            _sceneService = sceneService;
            _userService = userService;
            _websocketClient = websocketClient;
        }

        public int? SceneId { get; set; }
        public Action Saved;

        public override async Task InitializeAsync()
        {
            if (SceneId.HasValue)
            {
                Scene = _sceneService.GetSceneById(SceneId.Value);
                Devices = new ObservableCollection<SceneDeviceItemViewModel>(Scene.Devices.Select(x => new SceneDeviceItemViewModel(x)));

                foreach (var item in Devices)
                {
                    await _websocketClient.SubscribeToMessagesAsync(true, true, item.Device.Token, _logger);
                }

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
        public async Task EditDeviceAsync(SceneDeviceItemViewModel sc)
        {
            await _navigationService.NavigateAsync<SettingsDeviceViewModel>(x =>
            { 
                x.Device = sc;
                x.Deleted = () =>
                {
                    Devices.Remove(sc);
                };
            });
        }

        [RelayCommand]
        public async Task<SceneDevice> AddSceneDeviceAsync()
        {
            var point = new Point(50, 50);
            var datas = _userService.GetUserData().ToList();
            foreach (var item in Devices)
            {
                var existing = datas.FirstOrDefault(x => x.Mac.Equals(item.Device.Mac));
                if (existing != null)
                {
                    datas.Remove(existing);
                }
            }

            var viewModel = await _navigationService.ShowPopupAsync<SceneDeviceViewModel>(x => x.Datas = datas);

            if (!viewModel.Cancelled)
            {
                var sd = new SceneDevice
                {
                    Mac = viewModel.SelectedDevice.Mac,
                    SceneId = Scene.Id,
                    Type = viewModel.SelectedDevice.Type,
                    Token = viewModel.SelectedDevice.Token,
                    OriginalX = point.X,
                    OriginalY = point.Y
                };
                var vm = new SceneDeviceItemViewModel(sd);
                Devices.Add(vm);

                bool subscribed = await _websocketClient.SubscribeToMessagesAsync(true, true, sd.Token, _logger);
                if (!subscribed)
                {
                    Devices.Remove(vm);
                    await _navigationService.ShowPopupAsync<MessagePopupViewModel>(x => x.Message = "Can't subscribe to websocket. Check logs for more information.");
                    return null;
                }

                return sd;
            }

            return null;
        }

        [RelayCommand]
        public async Task SaveSceneAsync()
        {
            if (Scene == null)
                return;

            ScenePopupViewModel sc = new ScenePopupViewModel(_navigationService)
            {
                Name = Scene.Name
            };

            await _navigationService.ShowPopupAsync(sc);
            if (!sc.Cancelled)
            {
                Scene.Name = sc.Name;

                if (Devices.Any())
                {
                    Scene.Devices.Clear();
                    foreach (var item in Devices)
                    {
                        Scene.Devices.Add(item.Device);
                    }
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
                            stream.CopyTo(ms);
                            if (Scene == null)
                            {
                                Scene = new Scene();
                            }

                            Scene.Image = ms.ToArray();
                        }

                        SceneImage = ImageSource.FromStream(() => new MemoryStream(Scene.Image));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, "Can't load image.");
                await _navigationService.ShowPopupAsync<MessagePopupViewModel>(x => x.Message = "Can't load image. Check logs for more information.");
            }
        }

        [RelayCommand]
        public void ChangeMode()
        {
            GraphicMode = !GraphicMode;
        }     
        
        [RelayCommand]
        public async Task DeleteDeviceAsync(SceneDeviceItemViewModel device)
        {
            var vm = await _navigationService.ShowPopupAsync<MessagePopupViewModel>((Action<MessagePopupViewModel>)(x =>
            {
                x.Message = $"Do you really want to delete device {device.Device.Mac}?";
                x.IsConfirmation = true;
            }));

            if(!vm.Cancelled)
                Devices.Remove(device);
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
                    if (deviceResponse.IsState == true)
                    {
                        var state = JsonConvert.DeserializeObject<DeviceStateResponse>(message);
                        device.IsOnline = state.Data.IsOnline;
                    }
                    else
                    {
                        var data = JsonConvert.DeserializeObject<DeviceDataResponse>(message);

                        if (data != null)
                        {
                            device.IsOnline = true;
                        }
                    }
                }
            }
        }
    }
}
