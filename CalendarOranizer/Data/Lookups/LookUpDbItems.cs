using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CalendarOrganizer.DataAccess;
using CalendarOrganizer.Model;

namespace CalendarOrganizer.UI.Data.Lookups
{
    /// <summary>
    /// Implimentation of loading tasks from DB
    /// </summary>
    public class LookUpDbItems : ITaskDataLookUpService,
        ITaskTypeLookUpService, IDateTasksLookUpService
    {
        private Func<TaskOrganizeDbContext> _taskOrganizeDbContext;

        public LookUpDbItems(Func<TaskOrganizeDbContext> taskOrganizeDbCtx)
        {
            _taskOrganizeDbContext = taskOrganizeDbCtx;
        }

        #region Interface Implimentation
        public async Task<IEnumerable<LookUpItem>> LookUpTaskAsync()
        {
            using (var ctx = _taskOrganizeDbContext())
            {
                return await ctx.ToDoTasks.AsNoTracking()
                    .Select(task =>
                    new LookUpItem
                    {
                        Id = task.Id,
                        DisplayItem = task.TaskName
                    })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookUpItem>> LookUpTaskTypeAsync()
        {
            using (var ctx = _taskOrganizeDbContext())
            {
                return await ctx.ToDoTaskTypes.AsNoTracking()
                    .Select(task =>
                    new LookUpItem
                    {
                        Id = task.Id,
                        DisplayItem = task.Type
                    })
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<LookUpItem>> LookUpDateTasksAsync()
        {
            using (var ctx = _taskOrganizeDbContext())
            {
                return await ctx.ToDoDateTasks.AsNoTracking()
                    .Select(task =>
                    new LookUpItem
                    {
                        Id = task.Id,
                        DisplayItem = task.Date
                    })
                    .ToListAsync();
            }
        }
        #endregion
    }
}
