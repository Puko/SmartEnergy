using Newtonsoft.Json;

namespace SmartEnergy.Models
{
    public class RelaySettingsResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public RelayData Settings { get; set; }
    }
}
