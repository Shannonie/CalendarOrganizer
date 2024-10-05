using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace CalendarOrganizer.UI.View.Services
{
    public enum MessageDialogOptions
    {
        OK,
        Cancel
    }

    public class MessageDialogServices : IMessageDialogServices
    {
        private MetroWindow MetroWindow => (MetroWindow)App.Current.MainWindow;

        public async Task<MessageDialogOptions> ShowOkCancelDialogMessage(
            string text, string title)
        {
            MessageDialogResult response = await MetroWindow.ShowMessageAsync(
                title, text, MessageDialogStyle.AffirmativeAndNegative);
            return response == MessageDialogResult.Affirmative ?
                MessageDialogOptions.OK : MessageDialogOptions.Cancel;
        }

        public async void ShowInfoDialog(string text)
        {
            await MetroWindow.ShowMessageAsync("Info", text);
        }
    }
}
