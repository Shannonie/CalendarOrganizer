using CalendarOrganizer.Model;
using CalendarOrganizer.UI.Data.Repositories;
using CalendarOrganizer.UI.Event;
using CalendarOrganizer.UI.View.Services;
using CalendarOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalendarOrganizer.UI.ViewModel
{
    public class DateTasksViewModel : DetailViewModel, IDateTasksViewModel
    {
        private IDateTasksRepository _dateTasksRepository; //for database access (DateTasks)
        private DateTasksWrapper _dateTasks;
        private ToDoTask _selectedAddedTask;
        private ToDoTask _selectedAvailableTask;
        private List<ToDoTask> _allTasks;

        public DateTasksViewModel(
            IDateTasksRepository dateTasksRepository, IEventAggregator eventAggregator,
            IMessageDialogServices messageDialogServices)
            : base(eventAggregator, messageDialogServices)
        {
            _dateTasksRepository = dateTasksRepository;

            EventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(ExercuteAfterSavedTaskDetail);
            EventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(ExercuteAfterDeletedTaskDetail);
            AddTaskCommand = new DelegateCommand(ExecuteOnAddTask, OnAddTaskCanExecute);
            RemoveTaskCommand = new DelegateCommand(ExecuteOnRemoveTask, OnRemoveTaskCanExecute);

            AddedTasks = new ObservableCollection<ToDoTask>();
            AvailableTasks = new ObservableCollection<ToDoTask>();
        }

        #region Interface Implimentation
        public override async Task LoadAsync(int dateId)
        {
            DateTasks dateTasks = dateId > 0 ?
                await _dateTasksRepository.GetByIDAsync(dateId) : CreateNewDateTasks();
            Id = dateId;
            InitializeDateTasks(dateTasks);
            _allTasks = await _dateTasksRepository.GetAllTasksAsync();
            SetupPicklist();
        }

        public override void SetUpItemPropertyChanges()
        {
            DateTasksItem.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _dateTasksRepository.HasChanges();
                }

                if (e.PropertyName == nameof(DateTasksItem.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(DateTasksItem.Date))
                {
                    SetItemTitle(DateTasksItem.Date);
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public override void RaiseErrorIfItemCreated()
        {
            if (DateTasksItem.Id == 0) // for new created DateTasksItem, trigger validation
            {
                DateTasksItem.Date = "";
            }
        }
        #endregion

        #region UI property
        public DateTasksWrapper DateTasksItem
        {
            get { return _dateTasks; }
            set
            {
                _dateTasks = value;
                OnPropertyChanged();
            }
        }

        public ToDoTask SelectedAddedTask
        {
            get { return _selectedAddedTask; }
            set
            {
                _selectedAddedTask = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveTaskCommand).RaiseCanExecuteChanged();
            }
        }

        public ToDoTask SelectedAvailableTask
        {
            get { return _selectedAvailableTask; }
            set
            {
                _selectedAvailableTask = value;
                OnPropertyChanged();
                ((DelegateCommand)AddTaskCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<ToDoTask> AddedTasks { get; private set; }
        public ObservableCollection<ToDoTask> AvailableTasks { get; private set; }
        public ICommand AddTaskCommand { get; }
        public ICommand RemoveTaskCommand { get; }
        #endregion

        #region Icommand Implimentation
        private async void ExercuteAfterSavedTaskDetail(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(TaskViewModel))
            {
                //need to refresh because it's loading from catche
                await _dateTasksRepository.ReloadTaskAsync(args.Id);
                await GetAllTasksFromRepoAndUpdatePickList();
            }
        }

        private async void ExercuteAfterDeletedTaskDetail(AfterDetailDeletedEventArgs args)
        {
            if (args.ViewModelName == nameof(TaskViewModel))
            {
                await GetAllTasksFromRepoAndUpdatePickList();
            }
        }

        protected override async void ExecuteOnSaveDetail()
        {
            await _dateTasksRepository.SaveAsync();
            HasChanges = _dateTasksRepository.HasChanges();
            Id = DateTasksItem.Id;
            RaiseDetailSavedEvent(DateTasksItem.Id, $"{DateTasksItem.Date}");
        }

        protected override bool OnSaveDetailCanExecute()
        {
            bool taskStatus = DateTasksItem != null && !DateTasksItem.HasErrors;
            return taskStatus && HasChanges;
        }

        protected override async void ExecuteOnDeleteDetail()
        {
            MessageDialogOptions response = await MessageDialogServices.ShowOkCancelDialogMessage(
                $"Do you want to delete task {DateTasksItem.Date}?", "Question");

            if (response.Equals(MessageDialogOptions.OK))
            {
                _dateTasksRepository.Remove(DateTasksItem.Model);
                await _dateTasksRepository.SaveAsync();
                RaiseDetailDeletedEvent(DateTasksItem.Id);
            }
        }

        private void ExecuteOnAddTask()
        {
            ToDoTask taskToBeAdded = SelectedAvailableTask;
            AddedTasks.Add(taskToBeAdded);
            AvailableTasks.Remove(taskToBeAdded);
            DateTasksItem.Model.ToDoTasks.Add(taskToBeAdded);
            HasChanges = _dateTasksRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddTaskCanExecute()
        {
            return SelectedAvailableTask != null;
        }

        private void ExecuteOnRemoveTask()
        {
            ToDoTask taskToBeRemoved = SelectedAddedTask;
            AddedTasks.Remove(taskToBeRemoved);
            AvailableTasks.Add(taskToBeRemoved);
            DateTasksItem.Model.ToDoTasks.Remove(taskToBeRemoved);
            HasChanges = _dateTasksRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnRemoveTaskCanExecute()
        {
            return SelectedAddedTask != null;
        }
        #endregion

        private void InitializeDateTasks(DateTasks tasks)
        {
            DateTasksItem = new DateTasksWrapper(tasks);
            SetUpItemPropertyChanges();
            RaiseErrorIfItemCreated();
            SetItemTitle(DateTasksItem.Date);
        }

        private async Task GetAllTasksFromRepoAndUpdatePickList()
        {
            _allTasks = await _dateTasksRepository.GetAllTasksAsync();
            SetupPicklist();
        }

        private void SetupPicklist()
        {
            List<int> dateTasksIds = DateTasksItem.Model.ToDoTasks.Select(x => x.Id).ToList();
            List<ToDoTask> addedTasks = _allTasks.Where(task => dateTasksIds.Contains(task.Id))
                .OrderBy(task => task.TaskTime).ToList();
            List<ToDoTask> availableTasks = _allTasks.Except(addedTasks)
                .OrderBy(task => task.TaskTime).ToList();
            AddedTasks.Clear();
            AvailableTasks.Clear();
            foreach (ToDoTask task in addedTasks)
            {
                AddedTasks.Add(task);
            }
            foreach (ToDoTask task in availableTasks)
            {
                AvailableTasks.Add(task);
            }
            DateTasksItem.TimeFrom = AddedTasks.First().TaskTime;
            DateTasksItem.TimeTo = AddedTasks.Last().TaskTime;
        }

        private DateTasks CreateNewDateTasks()
        {
            DateTasks newDateTasks = new DateTasks()
            {
                TimeFrom = DateTime.Now.Date,
                TimeTo = DateTime.Now.Date
            };
            _dateTasksRepository.Add(newDateTasks);
            return newDateTasks;
        }
    }
}
