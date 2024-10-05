using Prism.Events;

namespace CalendarOrganizer.UI.Event
{
    public class AfterDetailClosedTabEvent : PubSubEvent<AfterDetailDeletedEventArgs>
    {
    }

    public class AfterDetailClosedEventArgs
    {
        public int? Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
