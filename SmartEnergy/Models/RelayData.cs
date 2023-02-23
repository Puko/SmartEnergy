using Newtonsoft.Json;

namespace SmartEnergy.Models
{
    public class RelayData
    {
        public RelaySettings R1 { get; set; }
        public RelaySettings R2 { get; set; }
        public RelaySettings R3 { get; set; }
        public RelaySettings R4 { get; set; }
    }
}
