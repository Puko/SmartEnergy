using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.Utils;

namespace SmartEnergy.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly SmartEnergyApiService _smartEnergyApi;
        private readonly UserService _userService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _pasword;

        public LoginViewModel(INavigationService navigationService, 
            SmartEnergyApiService smartEnergyApi, UserService userService)
        {
            _navigationService = navigationService;
            _smartEnergyApi = smartEnergyApi;
            _userService = userService;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (!await CheckConnection(_navigationService))
                return;
            
            if(!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Pasword))
            {
                var result = await Execute(() => _smartEnergyApi.LoginAsync(Username, Sha256Hash.ComputeSha256Hash(Pasword)));
                
                if(result.Succes)
                {
                    _userService.Login(result.Value);
                    await _navigationService.NavigateAsync<MainViewModel>(resetNavigation: true);
                }
                else
                {
                    await _navigationService.ShowPopupAsync<MessagePopupViewModel>(x => x.Message = "Zle meno alebo heslo.");
                }
            }
        }
    }
}
