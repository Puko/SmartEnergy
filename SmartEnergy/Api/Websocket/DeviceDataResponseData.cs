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

        public bool GetRelayState(int order)
        {
            switch (order)
            {
                case 1: return Relay1;
                case 2: return Relay2;
                case 3: return Relay3;
                case 4: return Relay4;
            }

            return false;
        }
    }
}
