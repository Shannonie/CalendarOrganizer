using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    /// <summary>
    /// Implimentation of loading/saving selected task from DB
    /// </summary>
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity>
       where TEntity : class
        where TContext : DbContext
    {
        protected readonly TContext _context;

        public GenericRepository(TContext context)
        {
            this._context = context;
        }

        public void Add(TEntity model)
        {
            _context.Set<TEntity>().Add(model);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<TEntity> GetByIDAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Remove(TEntity model)
        {
            _context.Set<TEntity>().Remove(model);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
