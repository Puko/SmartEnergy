using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class DeviceStateResponse : DeviceResponseBase
    {
        [JsonProperty("data")]
        public DeviceStateResponseData Data { get; set; }
    }
}
