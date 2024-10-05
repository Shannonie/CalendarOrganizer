using System.Windows;
using CalendarOrganizer.UI.ViewModel;
using MahApps.Metro.Controls;

namespace CalendarOrganizer.UI
{
    public partial class MainWindow : MetroWindow
    {
        private MainViewModel _mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            DataContext = _mainViewModel;
            Loaded += MainWindowLoaded;
        }

        private async void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                await _mainViewModel.LoadToDoTasksAsync();
            }
        }
    }
}
