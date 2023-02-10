using SmartEnergy.Database.Models;

namespace SmartEnergy.Database.Repositories
{
    public class SceneRepository : GenericRepository<Scene, int>
    {
        public SceneRepository(SmartEnergyDb context) : base(context)
        {
        }
    }
}
