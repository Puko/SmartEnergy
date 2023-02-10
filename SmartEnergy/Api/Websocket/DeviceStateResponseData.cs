using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class DeviceStateResponseData
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("fw")]
        public string Fw { get; set; }
        [JsonIgnore]
        public bool IsOnline => Status.Equals("ONLINE");
    }
}
