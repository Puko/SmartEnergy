using Microsoft.EntityFrameworkCore;
using SmartEnergy.Database.Models;

namespace SmartEnergy.Database.Repositories
{
    public class SceneDeviceRepository : GenericRepository<SceneDevice, int>
    {
        public SceneDeviceRepository(SmartEnergyDb context) : base(context)
        {
        }
    }
}
