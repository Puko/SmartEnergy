using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class SetRelayResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public bool Success => Status.Equals("OK");
    }
}
