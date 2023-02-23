using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Localization;
using System.Globalization;

namespace SmartEnergy.ViewModels
{
    public partial class StateViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ILogService _logService;
        private readonly WebsocketClient _client;

        [ObservableProperty]
        private bool _connected;

        private string _language;

        public StateViewModel(INavigationService navigationService, ILogService logService,
            WebsocketClient client)
        {
            _navigationService = navigationService;
            _logService = logService;
            _client = client;

            _connected = _client.IsConnected;

            var language = Preferences.Get("Language", string.Empty);

            _language = string.IsNullOrEmpty(language) ? "En" : "Sk";
        }

        public List<string> Languages => new List<string>
        {
            "En",
            "Sk"
        };

        public override Task Shown()
        {
            Connected = _client.IsConnected;
            return Task.CompletedTask;
        }

        public string Language
        {
            get { return _language; }
            set
            {
                if (SetProperty(ref _language, value))
                {
                    ChangeLanguage(_language);
                }
            }
        }

        public void ChangeLanguage(string language)
        {
            var switchToCulture = language
                .Equals("En", StringComparison.InvariantCultureIgnoreCase) ? "en-US" : "sk-SK";

            Preferences.Default.Set("Language", switchToCulture);

            Localization.SetCulture(new CultureInfo(switchToCulture));
        }

        [RelayCommand]
        public async Task ReconnectAsync()
        {
            if (!await CheckConnection(_navigationService))
                return;

            await _navigationService.ShowPopupAsyncWithoutResult<LoadingViewModel>(x => x.Message = $"Reconnecting...");

            _logService.Info("Reconnecting...");
            //reconnect logic is in app.xaml.cs
            _client.Dispose();
            await Task.Delay(1000);

            Connected = _client.IsConnected;

            _logService.Info($"Reconnection {(Connected ? "success" : "failed")}");
            await _navigationService.ClosePopupAsync();
        }

        private void Listen()
        {
            Task.Run(async () =>
            {
                if (!_client.IsConnected)
                    return;

                await foreach (var message in _client.ListenAsync(CancellationToken.None))
                {
                    if (message == WebsocketClient.DisconnectedMessage)
                    {
                        await _client.ReconnectAsync(_logService);
                        Listen();
                    }
                }
            });
        }



    }
}
