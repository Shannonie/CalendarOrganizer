using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarOrganizer.Model
{
    public class DateTasks
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Date { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime TimeFrom { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime TimeTo { get; set; }

        public ICollection<ToDoTask> ToDoTasks { get; set; }

        public DateTasks()
        {
            ToDoTasks = new Collection<ToDoTask>();
        }
    }
}
