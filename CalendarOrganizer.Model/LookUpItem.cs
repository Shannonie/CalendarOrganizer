namespace CalendarOrganizer.Model
{
    public class LookUpItem
    {
        public int Id { get; set; }
        public string DisplayItem { get; set; }
    }

    public class LookUpNullItem : LookUpItem
    {
        public new int? Id { get { return null; } }
    }
}
