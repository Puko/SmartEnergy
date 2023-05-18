using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartEnergy.Models;

namespace SmartEnergy.Database.Models
{
    public class Device : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool Owner { get; set; }
        public string Mac { get; set; }
        public int Type { get; set; }
        public string Token { get; set; }

        [JsonProperty("phase_use")]
        public int PhaseUse { get; set; }
        public bool Lora { get; set; }
        public Nilm NILM { get; set; }

        public int UserInformationId { get; set; }
        public UserInformation UserInformation { get; set; }
    }

}
