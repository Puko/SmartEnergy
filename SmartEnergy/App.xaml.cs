using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Localization;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;
using SmartEnergy.Views;
using System.Globalization;

namespace SmartEnergy;

public partial class App : Application
{
    private readonly INavigationService _navigationService;
    private readonly ILogService _logService;
    private readonly UserService _userService;
    private readonly WebsocketClient _websocketClient;

    public App(INavigationService navigationService, ILogService logService,
        UserService userService, WebsocketClient websocketClient)
    {
        InitializeComponent();

        _navigationService = navigationService;
        _logService = logService;
        _userService = userService;
        _websocketClient = websocketClient;

        MainPage = new NavigationPage(new LoadingView());
    }

    protected override async void OnStart()
    {
        var language = Preferences.Get("Language", string.Empty);
        LocalizationResourceManager.Instance.SetCulture(string.IsNullOrEmpty(language) ? new CultureInfo("en-US") : new CultureInfo("sk-Sk"));

        try
        {
            _logService.Info("Connecting to websocket...");
            
            await _websocketClient.ConnectAsync(_logService);
            if(_websocketClient.IsConnected)
               Task.Run(async () => await ListenAsync());
            else
                Reconnect();

            _logService.Info("Connection success...");
        }
        catch (Exception e)
        {
            _logService.Exception(e, "Can't connect to websocket!");
        }

        if (_userService.IsLogged())
            await _navigationService.NavigateAsync<MainViewModel>(resetNavigation: true);
        else
            await _navigationService.NavigateAsync<LoginViewModel>(resetNavigation: true);
    }

    private void Reconnect()
    {
        Task.Run(async () =>
        {
            if (!_websocketClient.IsConnected)
            {
                await _websocketClient.ReconnectAsync(_logService);
                await Task.Delay(2000);

                Reconnect();
            }
            else
            {
                await ListenAsync();
            }
        });
    }

    private async Task ListenAsync()
    {
        await foreach (var message in _websocketClient.ListenAsync(CancellationToken.None))
        {
            if (message == WebsocketClient.DisconnectedMessage)
            {
                Reconnect();
            }
        }
    }
}
