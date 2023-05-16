using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartEnergy.Database.Models
{
    public class Scene : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string User { get; set; }
        public List<SceneDevice> Devices { get; set; } = new List<SceneDevice>();

        public double OriginalWidth { get; set; }
        public double OriginalHeight { get; set; }

        [NotMapped]
        public int DevicesCount { get; set; }
    }
}
