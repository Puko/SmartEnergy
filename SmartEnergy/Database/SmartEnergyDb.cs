using Microsoft.EntityFrameworkCore;
using SmartEnergy.Database.Models;

namespace SmartEnergy.Database
{
    public class SmartEnergyDb : DbContext
    {
        public SmartEnergyDb(DbContextOptions options) 
            : base(options)
        {

        }

        public DbSet<Scene> Scenes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserInformation> UserInformation { get; set; }
        public DbSet<Models.Device> UserDatas { get; set; }
        public DbSet<SceneDevice> SceneDevices { get; set; }
    }
}
