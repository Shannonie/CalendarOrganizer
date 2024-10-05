using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Prism.Events;
using CalendarOrganizer.Model;
using CalendarOrganizer.UI.Event;
using CalendarOrganizer.UI.Data.Lookups;

namespace CalendarOrganizer.UI.ViewModel
{
    /// <summary>
    /// Implimentation of updating tasks from:
    ///     1.DB 
    ///     2.selected task changes
    /// to MainViewModel
    /// </summary>
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private ITaskDataLookUpService _lookUpDataService; //for database:ToDoTasks access
        private IDateTasksLookUpService _lookUpDateTasksService; //for database:ToDoDateTasks access
        private IEventAggregator _eventAggregator; //for viewModel communication

        public NavigationViewModel(ITaskDataLookUpService taskLookUpDataService,
            IDateTasksLookUpService lookUpDateTasksService,
            IEventAggregator eventAggregator)
        {
            _lookUpDataService = taskLookUpDataService;
            _lookUpDateTasksService = lookUpDateTasksService;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(ExercuteAfterSavedDetail);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(ExercuteAfterDeletedDetail);

            ToDoTasks = new ObservableCollection<NavigationItemViewModel>();
            DatesTasks = new ObservableCollection<NavigationItemViewModel>();
        }

        #region Interface Implimentation
        public async Task LoadAsync()
        {
            IEnumerable<LookUpItem> lookUpItems = await _lookUpDataService.LookUpTaskAsync();
            ToDoTasks.Clear();
            foreach (LookUpItem item in lookUpItems)
            {
                ToDoTasks.Add(new NavigationItemViewModel(item.Id, item.DisplayItem,
                    nameof(TaskViewModel), _eventAggregator));
            }

            IEnumerable<LookUpItem> lookUpDateItems = await _lookUpDateTasksService.LookUpDateTasksAsync();
            DatesTasks.Clear();
            foreach (LookUpItem item in lookUpDateItems)
            {
                DatesTasks.Add(new NavigationItemViewModel(item.Id, item.DisplayItem,
                    nameof(DateTasksViewModel), _eventAggregator));
            }
        }
        #endregion

        #region UI property
        public ObservableCollection<NavigationItemViewModel> ToDoTasks { get; set; }
        public ObservableCollection<NavigationItemViewModel> DatesTasks { get; set; }
        #endregion

        #region Icommand Implimentation
        private void ExercuteAfterSavedDetail(AfterDetailSavedEventArgs args)
        {
            NavigationItemViewModel toBeSavedTask = null;
            switch (args.ViewModelName)
            {
                case nameof(TaskViewModel):
                    toBeSavedTask = SaveDetail(ToDoTasks, args);
                    break;
                case nameof(DateTasksViewModel):
                    toBeSavedTask = SaveDetail(DatesTasks, args);
                    break;
            }
        }

        private void ExercuteAfterDeletedDetail(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(TaskViewModel):
                    DeleteDetail(ToDoTasks, args);
                    break;
                case nameof(DateTasksViewModel):
                    DeleteDetail(DatesTasks, args);
                    break;
            }
        }
        #endregion

        private NavigationItemViewModel SaveDetail(
            ObservableCollection<NavigationItemViewModel> items,
            AfterDetailSavedEventArgs args)
        {
            NavigationItemViewModel toBeSavedItem =
                items.SingleOrDefault(it => it.Id == args.Id);
            if (toBeSavedItem == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember,
                    nameof(args.ViewModelName), _eventAggregator));
            }
            else
            {
                toBeSavedItem.DisplayMember = args.DisplayMember;
            }

            return toBeSavedItem;
        }

        private void DeleteDetail(
            ObservableCollection<NavigationItemViewModel> items,
            AfterDetailDeletedEventArgs args)
        {
            NavigationItemViewModel toBeDeletedItem =
                items.SingleOrDefault(it => it.Id == args.Id);
            if (toBeDeletedItem != null)
            {
                items.Remove(toBeDeletedItem);
            }
        }
    }
}
