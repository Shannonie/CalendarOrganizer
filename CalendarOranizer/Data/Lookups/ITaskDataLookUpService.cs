using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarOrganizer.Model;

namespace CalendarOrganizer.UI.Data.Lookups
{
    public interface ITaskDataLookUpService
    {
        Task<IEnumerable<LookUpItem>> LookUpTaskAsync();
    }
}