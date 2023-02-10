using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;
using SmartEnergy.Views;
using System.Diagnostics;

namespace SmartEnergy;

public partial class App : Application
{
    private readonly INavigationService _navigationService;
    private readonly UserService _userService;
    private readonly WebsocketClient _websocketClient;

    public App(INavigationService navigationService, UserService userService, WebsocketClient websocketClient)
    {
        InitializeComponent();

        _navigationService = navigationService;
        _userService = userService;
        _websocketClient = websocketClient;

        MainPage = new NavigationPage(new LoadingView());
    }

    protected override async void OnStart()
    {
        try
        {
            await _websocketClient.ConnectAsync();
            Listen();
        }
        catch 
        {
            //todo log
        }

        if (_userService.IsLogged())
            await _navigationService.NavigateAsync<MainViewModel>(resetNavigation: true);
        else
            await _navigationService.NavigateAsync<LoginViewModel>(resetNavigation: true);
    }

    private void Listen()
    {
        Task.Run(async () =>
        {
            await foreach (var message in _websocketClient.ListenAsync(CancellationToken.None))
            {
                if (message == WebsocketClient.DisconnectedMessage)
                {
                    await _websocketClient.ReconnectAsync();
                    Listen();
                }
            }
        });
    }
}
