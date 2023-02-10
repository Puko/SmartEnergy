using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class DeviceResponseBase
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }
        [JsonProperty("device")]
        public string Device { get; set; }

        public bool? IsState => Topic?.ToLowerInvariant().Equals("state");
    }
}
