using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class DeviceDataResponseData
    {
        [JsonProperty("relay_1")]
        public bool Relay1 { get; set; }
        [JsonProperty("relay_2")]
        public bool Relay2 { get; set; }
        [JsonProperty("relay_3")]
        public bool Relay3 { get; set; }
        [JsonProperty("relay_4")]
        public bool Relay4 { get; set; }
    }
}
