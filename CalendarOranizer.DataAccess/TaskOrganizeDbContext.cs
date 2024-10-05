using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using CalendarOrganizer.Model;

namespace CalendarOrganizer.DataAccess
{
    public class TaskOrganizeDbContext : DbContext
    {
        public DbSet<ToDoTask> ToDoTasks { get; set; }
        public DbSet<ToDoTaskType> ToDoTaskTypes { get; set; }
        public DbSet<Contact> ToDoTaskContacts { get; set; }
        public DbSet<DateTasks> ToDoDateTasks { get; set; }

        public TaskOrganizeDbContext() : base("TaskOrganizeDb")
        { }

        /// <summary>
        /// maps the entities by the assembly
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
