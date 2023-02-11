using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SmartEnergy.Database.Models
{
    public class UserInformation : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Status { get; set; }
        [JsonProperty("data")]
        public IList<Device> Devices { get; set; }
        public User User { get; set; }
    }
}
