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

        public int UserInformationId { get; set; }
        public UserInformation UserInformation { get; set; }
    }

}
