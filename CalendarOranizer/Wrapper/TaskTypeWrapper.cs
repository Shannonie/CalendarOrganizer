using CalendarOrganizer.Model;

namespace CalendarOrganizer.UI.Wrapper
{
    public class TaskTypeWrapper : ModelWrapper<ToDoTaskType>
    {
        public TaskTypeWrapper(ToDoTaskType model) : base(model)
        { }

        public int Id { get { return Model.Id; } }

        public string Type
        {
            get { return GetValue<string>(nameof(Type)); }
            set
            {
                SetValue(value, nameof(Type));
            }
        }
    }
}
