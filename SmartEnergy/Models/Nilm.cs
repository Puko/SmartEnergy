using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartEnergy.Database.Models;
using Device = SmartEnergy.Database.Models.Device;

namespace SmartEnergy.Models
{
    public class Nilm : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Process { get; set; }
        public bool AutoLearn { get; set; }

        public int UserDataId { get; set; }
        public Device UserData { get; set; }
    }
}
