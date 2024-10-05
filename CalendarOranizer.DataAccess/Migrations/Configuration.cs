namespace CalendarOranizerDataAccess.Migrations
{
    using CalendarOrganizer.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration :
        DbMigrationsConfiguration<CalendarOrganizer.DataAccess.TaskOrganizeDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CalendarOrganizer.DataAccess.TaskOrganizeDbContext context)
        {
            context.ToDoTasks.AddOrUpdate(
                task => task.TaskName,
                new ToDoTask
                {
                    TaskName = "Brief Meeting",
                    TaskTime = new DateTime(2024, 1, 25, 8, 30, 0),
                    TaskDescription = "at room D4",
                    TaskType = new ToDoTaskType { Type = "Company" }
                },
                new ToDoTask
                {
                    TaskName = "Get Hair Trimmed",
                    TaskTime = new DateTime(2024, 1, 30, 10, 30, 0),
                    TaskType = new ToDoTaskType { Type = "Personal" }
                },
                new ToDoTask
                {
                    TaskName = "Pick Up Kids",
                    TaskTime = new DateTime(2024, 1, 30, 4, 30, 0),
                    TaskDescription = "Katy and Brody",
                    TaskType = new ToDoTaskType { Type = "Family" }
                },
                new ToDoTask
                {
                    TaskName = "Freddy's Birthday",
                    TaskTime = new DateTime(2024, 2, 5, 18, 30, 0),
                    TaskDescription = "At Freddy's",
                    TaskType = new ToDoTaskType { Type = "Friend" }
                }
                );

            context.ToDoTaskTypes.AddOrUpdate(
                type => type.Type,
                new ToDoTaskType { Type = "Family" },
                new ToDoTaskType { Type = "Company" },
                new ToDoTaskType { Type = "Friend" },
                new ToDoTaskType { Type = "Personal" },
                new ToDoTaskType { Type = "Other" }
                );

            context.SaveChanges(); //Save ToDoTasks's updates before ToDoTaskContacts

            context.ToDoTaskContacts.AddOrUpdate(
                contact => contact.Name,
                new Contact
                {
                    Name = "Julia",
                    PhoneNumber = "123456789",
                    TaskId = context.ToDoTasks.First().Id
                }
                );

            context.ToDoDateTasks.AddOrUpdate(
                contact => contact.Date,
                new DateTasks
                {
                    Date = "2024-1-30",
                    TimeFrom = new DateTime(2024, 1, 30, 10, 30, 0),
                    TimeTo = new DateTime(2024, 1, 30, 17, 00, 0),
                    ToDoTasks = new List<ToDoTask>()
                    {
                        context.ToDoTasks.Single(task=> task.TaskName=="Get Hair Trimmed"),
                        context.ToDoTasks.Single(task=> task.TaskName=="Pick Up Kids")
                    }
                }
                );
        }
    }
}
