using System.ComponentModel.DataAnnotations;

namespace CalendarOrganizer.Model
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        public string Name { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public int TaskId { get; set; }
        public ToDoTask Task { get; set; }
    }
}
