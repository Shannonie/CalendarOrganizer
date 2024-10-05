using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIDAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task SaveAsync();
        bool HasChanges();
        void Add(T task);
        void Remove(T task);
    }
}
