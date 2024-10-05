using CalendarOrganizer.Model;
using System;

namespace CalendarOrganizer.UI.Wrapper
{
    public class DateTasksWrapper : ModelWrapper<DateTasks>
    {
        public DateTasksWrapper(DateTasks model) : base(model)
        {
        }

        public int Id
        {
            get { return Model.Id; }
        }

        public string Date
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public DateTime TimeFrom
        {
            get { return GetValue<DateTime>(); }
            set
            {
                SetValue(value);
                if (TimeTo < TimeFrom)
                {
                    TimeTo = TimeFrom;
                }
            }
        }

        public DateTime TimeTo
        {
            get { return GetValue<DateTime>(); }
            set
            {
                SetValue(value);
                if (TimeTo < TimeFrom)
                {
                    TimeFrom = TimeTo;
                }
            }
        }
    }
}
