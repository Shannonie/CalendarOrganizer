using CalendarOrganizer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.Data.Lookups
{
    public interface IDateTasksLookUpService
    {
        Task<IEnumerable<LookUpItem>> LookUpDateTasksAsync();
    }
}