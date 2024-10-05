using CalendarOrganizer.Model;
using System;
using System.Collections.Generic;

namespace CalendarOrganizer.UI.Wrapper
{
    public class TaskWrapper : ModelWrapper<ToDoTask>
    {
        public TaskWrapper(ToDoTask model) : base(model)
        { }

        public int Id { get { return Model.Id; } }

        public string TaskName
        {
            get { return GetValue<string>(nameof(TaskName)); }
            set
            {
                SetValue(value, nameof(TaskName));
            }
        }

        public int? TaskTypeId
        {
            get { return GetValue<int?>(); }
            set { SetValue(value); }
        }

        public DateTime TaskTime
        {
            get { return GetValue<DateTime>(nameof(TaskTime)); }
            set
            {
                SetValue(value, nameof(TaskTime));
            }
        }

        public string TaskDescription
        {
            get { return GetValue<string>(nameof(TaskDescription)); }
            set
            {
                SetValue(value, nameof(TaskDescription));
            }
        }

        // Custom validation
        protected override IEnumerable<string> ValidationProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(TaskName):
                    if (TaskName.Length <= 5)
                    {
                        yield return "TaskName should have longer name!";
                    }
                    break;
            }
        }
    }
}
