using Newtonsoft.Json;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;
using SmartEnergy.Services;
using SmartEnergy.ViewModels;

namespace SmartEnergy.Extensions
{
    public static class ResponseExtenstions
    {
        public static async Task<bool> HandleExpiredTokenAsync(this string message, INavigationService navigationService, 
            UserService userService, WebsocketClient client)
        {
            var response = JsonConvert.DeserializeObject<Response>(message);
            if (response?.IsTokenExpired == true)
            {
                await client.UnsubscribeAll();
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await navigationService.ShowPopupAsync<MessagePopupViewModel>(x => { x.Message = "Device token is expired. Please login again"; });
                    userService.Logout();
                    await navigationService.NavigateAsync<LoginViewModel>(resetNavigation: true);
                });

                return true;
            }

            return false;
        }
    }
}
