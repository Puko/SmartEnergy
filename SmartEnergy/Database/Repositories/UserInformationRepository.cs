using Microsoft.EntityFrameworkCore;
using SmartEnergy.Database.Models;

namespace SmartEnergy.Database.Repositories
{
    public class UserInformationRepository : GenericRepository<UserInformation, int>
    {
        public UserInformationRepository(SmartEnergyDb context) : base(context)
        {
        }
    }
}
