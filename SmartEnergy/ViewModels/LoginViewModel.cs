using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartEnergy.Database;
using SmartEnergy.Database.Models;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.Utils;

namespace SmartEnergy.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly SmartEnergyDb _smartEnergyDb;
        private readonly SmartEnergyApiService _smartEnergyApi;
        private readonly UserService _userService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _pasword;

        public LoginViewModel(INavigationService navigationService, SmartEnergyDb smartEnergyDb,
            SmartEnergyApiService smartEnergyApi, UserService userService)
        {
            _navigationService = navigationService;
            _smartEnergyDb = smartEnergyDb;
            _smartEnergyApi = smartEnergyApi;
            _userService = userService;
        }

        [RelayCommand]
        public async Task ReinitializeDatabaseAsync()
        {
            try
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, @"SmartEnergy.db");

                await _smartEnergyDb.Database.EnsureDeletedAsync();
                await _smartEnergyDb.Database.EnsureCreatedAsync();

                await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = $"Database recreated. Db path: {dbPath}");
            }
            catch (Exception e)
            {
                await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = e.Message);
            }
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (!await CheckConnection(_navigationService))
                return;

            try
            {
                if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Pasword))
                {
                    var result = await Execute(() => _smartEnergyApi.LoginAsync(Username, Sha256Hash.ComputeSha256Hash(Pasword)));

                    if (result.Succes)
                    {
                        await SecureStorage.SetAsync("UserName", Username);
                        await SecureStorage.SetAsync("Password", Sha256Hash.ComputeSha256Hash(Pasword));

                        _userService.Login(result.Value);
                        await _navigationService.NavigateAsync<MainViewModel>(resetNavigation: true);
                    }
                    else
                    {
                        await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = Localization["BadUsernameOrPasswordMessage"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                await _navigationService.ShowPopupAsync<InfoViewModel>(x => x.Message = e.InnerException.Message);
            }
          
        }
    }
}
