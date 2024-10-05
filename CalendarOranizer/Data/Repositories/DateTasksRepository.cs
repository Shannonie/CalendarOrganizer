using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using CalendarOrganizer.DataAccess;
using CalendarOrganizer.Model;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public class DateTasksRepository : GenericRepository<DateTasks, TaskOrganizeDbContext>,
        IDateTasksRepository
    {
        public DateTasksRepository(TaskOrganizeDbContext context)
           : base(context)
        { }

        public override async Task<DateTasks> GetByIDAsync(int id)
        {
            return await _context.ToDoDateTasks
                .Include(date => date.ToDoTasks)
                .SingleAsync(date => date.Id == id);
        }

        public async Task<List<ToDoTask>> GetAllTasksAsync()
        {
            return await _context.Set<ToDoTask>().ToListAsync();
        }

        public async Task ReloadTaskAsync(int taskId)
        {
            DbEntityEntry<ToDoTask> dbEntityEntry = _context.ChangeTracker.Entries<ToDoTask>()
                .SingleOrDefault(db => db.Entity.Id == taskId);
            if (dbEntityEntry != null)
            {
                await dbEntityEntry.ReloadAsync();
            }
        }
    }
}
