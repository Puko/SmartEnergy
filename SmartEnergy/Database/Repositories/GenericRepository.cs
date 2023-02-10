using Microsoft.EntityFrameworkCore;
using OfferingSolutions.GenericEFCore.RepositoryContext;
using SmartEnergy.Database.Models;

namespace SmartEnergy.Database.Repositories
{
    public abstract class GenericRepository<T, TPrimaryKey> : GenericRepositoryContext<T> where T : class, IEntity<TPrimaryKey>
    {
        private readonly DbContext _context;

        public GenericRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public void Delete(TPrimaryKey primaryKey)
        {
            var toDelete = GetSingle(x => Equals(primaryKey, x.Id));
            if (toDelete != null)
            {
                RemoveFromChangeTracker(toDelete);
                Delete(toDelete);
            }
        }

        public bool IsTracked(T entity)
        {
            return _context.Set<T>().Local.Any(e => e.Id.Equals(entity.Id));
        }

        public bool RemoveFromChangeTracker(T entity)
        {
            var entityToRemove = _context.Set<T>().Local.FirstOrDefault(e => e.Id.Equals(entity.Id));

            if (entityToRemove != null)
                return _context.Set<T>().Local.Remove(entityToRemove);

            return false;
        }
    }
}
