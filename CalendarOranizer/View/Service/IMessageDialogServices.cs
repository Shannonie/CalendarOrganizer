using System.Threading.Tasks;

namespace CalendarOrganizer.UI.View.Services
{
    public interface IMessageDialogServices
    {
        Task<MessageDialogOptions> ShowOkCancelDialogMessage(string text, string title);
        void ShowInfoDialog(string text);
    }
}