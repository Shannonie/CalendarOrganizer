using System;
using System.ComponentModel.DataAnnotations;

namespace CalendarOrganizer.Model
{
    public class ToDoTaskType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        public string Type { get; set; }
    }
}
