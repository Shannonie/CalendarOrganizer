using CalendarOrganizer.Model;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Repositories
{
    public interface ITaskRepository : IGenericRepository<ToDoTask>
    {
        Task<bool> IsInDateTodos(int id);
        //Task<bool> IsInTaskType(int id);
        void RemoveContact(Contact model);
    }
}
