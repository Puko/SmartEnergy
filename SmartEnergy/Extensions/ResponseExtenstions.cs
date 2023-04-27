using Newtonsoft.Json;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;

namespace SmartEnergy.Extensions
{
    public static class ResponseExtenstions
    {
        public static async Task<bool> HandleExpiredTokenAsync(this string message, INavigationService navigationService, ILogService logService,
            UserService userService, WebsocketClient client , SmartEnergyApiService smartEnergyApi)
        {
            var response = JsonConvert.DeserializeObject<Response>(message);
            if (response?.IsTokenExpired == true)
            {
                await client.UnsubscribeAll();
                var userName = await SecureStorage.GetAsync("UserName");
                var passWord = await SecureStorage.GetAsync("Password");
                var data = await smartEnergyApi.LoginAsync(userName, passWord);

                if (data.Succes)
                {
                    userService.Login(data.Value);
                    foreach (var device in data.Value.Devices)
                    {
                        await client.SubscribeToMessagesAsync(true, true, device.Token, logService);
                    }
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        userService.Logout();
                        await navigationService.NavigateAsync<LoginViewModel>(resetNavigation: true);
                    });
                }
                return true;
            }

            return false;
        }
    }
}
