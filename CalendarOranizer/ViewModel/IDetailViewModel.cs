using System.Threading.Tasks;

namespace CalendarOrganizer.UI.ViewModel
{
    public interface IDetailViewModel
    {
        int Id { get; }
        string Title { get; }
        bool HasChanges { get; }
        void SetItemTitle(string title);
        Task LoadAsync(int id);
        void SetUpItemPropertyChanges();
        void RaiseErrorIfItemCreated();
    }
}