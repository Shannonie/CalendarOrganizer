using Prism.Events;

namespace CalendarOrganizer.UI.Event
{
    public class OpenDetailEvent:PubSubEvent<OpenDetailEventArgs>
    {
    }

    public class OpenDetailEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }   
    }
}
