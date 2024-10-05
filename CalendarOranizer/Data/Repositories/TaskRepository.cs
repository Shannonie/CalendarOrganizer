using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CalendarOrganizer.DataAccess;
using CalendarOrganizer.Model;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public class TaskRepository :
        GenericRepository<ToDoTask, TaskOrganizeDbContext>, ITaskRepository
    {
        public TaskRepository(TaskOrganizeDbContext context)
           : base(context)
        { }

        public override async Task<ToDoTask> GetByIDAsync(int id)
        {
            return await _context.ToDoTasks
                .Include(task => task.TaskContacts)
                .SingleAsync(task => task.Id == id);
        }

        public async Task<bool> IsInDateTodos(int id)
        {
            return await _context.ToDoDateTasks.AsNoTracking()
                .Include(task => task.ToDoTasks)
                .AnyAsync(task => task.ToDoTasks.Any(t => t.Id == id));
        }

        public void RemoveContact(Contact model)
        {
            _context.ToDoTaskContacts.Remove(model);
        }
    }
}
