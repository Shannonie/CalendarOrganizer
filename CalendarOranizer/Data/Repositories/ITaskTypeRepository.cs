using CalendarOrganizer.Model;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public interface ITaskTypeRepository : IGenericRepository<ToDoTaskType>
    {
        Task<bool> IsReferencedByTodoTask(int taskTypeId);
    }
}
