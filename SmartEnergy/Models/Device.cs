using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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



    public class Rootobject
    {
        public string status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public R1 R1 { get; set; }
        public R2 R2 { get; set; }
        public R3 R3 { get; set; }
        public R4 R4 { get; set; }
    }

    public class R1
    {
        public string name { get; set; }
        public int power { get; set; }
    }

    public class R2
    {
        public string name { get; set; }
        public int power { get; set; }
    }

    public class R3
    {
        public string name { get; set; }
        public int power { get; set; }
    }

    public class R4
    {
        public string name { get; set; }
        public int power { get; set; }
    }

}
