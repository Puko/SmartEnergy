using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{
    public class Response
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public bool IsTokenExpired => Code == 714;
        public bool OfflineDevice => Code == 604;
    }

}
