using Newtonsoft.Json;
using SmartEnergy.Api.Websocket;
using SmartEnergy.Interfaces;

namespace SmartEnergy.Extensions
{
    public static class WebsocketExtensions
    {
        private static List<string> _subscribedDevices = new List<string>();

        public static async Task UnsubscribeAll(this WebsocketClient client)
        {
            foreach (var item in _subscribedDevices)
            {
                await client.SendStringAsync(JsonConvert.SerializeObject(new DeviceStateRequest
                {
                    DeviceToken = item,
                    Data = false,
                    State = false
                }), CancellationToken.None);
            }

            _subscribedDevices.Clear();
        }

        public static async Task<bool> SubscribeToMessagesAsync(this WebsocketClient client, bool enableState, 
            bool enableData, string deviceToken, ILogService logService)
        {
            try
            {
                if (!client.IsConnected)
                    await client.ReconnectAsync();

                if (_subscribedDevices.Any(x => x.Equals(deviceToken)))
                    return true;


                _subscribedDevices.Add(deviceToken);
                await client.SendStringAsync(JsonConvert.SerializeObject(new DeviceStateRequest
                {
                    DeviceToken = deviceToken,
                    Data = enableData,
                    State = enableState
                }), CancellationToken.None);

                return true;
            }
            catch (Exception ex) 
            {
                logService.Exception(ex, "Failed to send websocket message.");
                return false;
            }
        }
    }
}
