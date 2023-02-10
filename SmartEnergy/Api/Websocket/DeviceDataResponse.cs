using Newtonsoft.Json;

namespace SmartEnergy.Api.Websocket
{

    public class DeviceDataResponse : DeviceResponseBase
    {

        [JsonProperty("data")]
        public DeviceDataResponseData Data { get; set; }
    }
}
