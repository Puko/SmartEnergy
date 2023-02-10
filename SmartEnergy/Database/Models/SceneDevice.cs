using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartEnergy.Database.Models
{
    public class SceneDevice : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Mac { get; set; }
        public string Token { get; set; }
        public int Type { get; set; }

        public double OriginalX { get; set; }
        public double OriginalY { get; set; }

        public int SceneId { get; set; }
        public Scene Scene { get; set; }

    }
}
