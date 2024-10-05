using CalendarOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public interface IDateTasksRepository : IGenericRepository<DateTasks>
    {
        Task<List<ToDoTask>> GetAllTasksAsync();
        Task ReloadTaskAsync(int taskId);
    }
}