using CalendarOrganizer.DataAccess;
using CalendarOrganizer.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public class TaskTypeRepository :
        GenericRepository<ToDoTaskType, TaskOrganizeDbContext>, ITaskTypeRepository
    {
        public TaskTypeRepository(TaskOrganizeDbContext context) : base(context)
        {
            
        }

        #region Interface Implimentation
        public async Task<bool> IsReferencedByTodoTask(int taskTypeId)
        {
            return await _context.ToDoTasks.AsNoTracking()
                .AnyAsync(task => task.TaskTypeId == taskTypeId);
        }
        #endregion
    }
}
