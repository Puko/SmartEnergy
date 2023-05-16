using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartEnergy.Database.Models
{
    public class User : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Token { get; set; }
        [NotMapped]
        public  string Name => Token.Split("|")[0];

        public UserInformation UserInformation { get; set; }
        public int UserInformationId { get; set; }
    }
}
