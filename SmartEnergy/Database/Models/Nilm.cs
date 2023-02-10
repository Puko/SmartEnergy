using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartEnergy.Database.Models
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
