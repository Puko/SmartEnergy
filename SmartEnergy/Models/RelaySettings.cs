using Newtonsoft.Json;

namespace SmartEnergy.Models
{
    public class RelaySettings
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("power")]
        public int Power { get; set; }
    }
}
