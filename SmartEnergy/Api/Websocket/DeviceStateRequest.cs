using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class DeviceStateRequest
    {
        [JsonProperty("deviceToken")]
        public string DeviceToken { get; set; }
        [JsonProperty("state")]
        public bool State { get; set; }
        [JsonProperty("data")]
        public bool Data { get; set; }
    }
}
