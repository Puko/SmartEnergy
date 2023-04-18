using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Extensions;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;

namespace SmartEnergy.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly WebsocketClient _client;
        private readonly UserService _userService;
        private readonly INavigationService _navigationService;

        public MainViewModel(SceneListViewModel sceneViewModel, StateViewModel stateViewModel,
            LogsViewModel logsViewModel, WebsocketClient client,
            UserService userService, INavigationService navigationService)
        {
            SceneViewModel = sceneViewModel;
            StateViewModel = stateViewModel;
            LogsViewModel = logsViewModel;

            _client = client;
            _userService = userService;
            _navigationService = navigationService;
        }

        public SceneListViewModel SceneViewModel { get; }
        public StateViewModel StateViewModel { get; }
        public LogsViewModel LogsViewModel { get; }

        [RelayCommand]
        public async Task LogoutAsync()
        {
            await _client.UnsubscribeAll();
            _userService.Logout();
            await _navigationService.NavigateAsync<LoginViewModel>(resetNavigation: true);
        }
    }
}
