using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarOrganizer.Model
{
    public class ToDoTask
    {
        public ToDoTask()
        {
            TaskContacts = new Collection<Contact>();
            DateTaskEvents = new Collection<DateTasks>();
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string TaskName { get; set; } = "";
        [StringLength(250)]
        public string TaskDescription { get; set; } = "";
        [Column(TypeName = "datetime2")]
        public DateTime TaskTime { get; set; }
        public int? TaskTypeId { get; set; }
        public ToDoTaskType TaskType { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Contact> TaskContacts { get; set; }
        public ICollection<DateTasks> DateTaskEvents { get; set; }
    }
}
