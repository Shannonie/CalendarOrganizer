using CalendarOrganizer.Model;
using CalendarOrganizer.UI.Data.Repositories;
using CalendarOrganizer.UI.View.Services;
using CalendarOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalendarOrganizer.UI.ViewModel
{
    public class TaskTypeViewModel : DetailViewModel
    {
        private ITaskTypeRepository _taskTypeRepository; //for database access (ToDoTaskType)
        private TaskTypeWrapper _selectedTaskType; //for database access (ToDoTaskType)
        
        public TaskTypeViewModel(IEventAggregator eventAggregator,
            ITaskTypeRepository taskTypeRepository,
            IMessageDialogServices messageDialogServices)
            : base(eventAggregator, messageDialogServices)
        {
            _taskTypeRepository = taskTypeRepository;

            AddCommand = new DelegateCommand(ExecuteOnAddTaskType);
            RemoveCommand = new DelegateCommand(ExecuteOnRemoveTaskType, OnRemoveTaskTypeCanExecute);

            Title = "Task Type";

            TaskTypes = new ObservableCollection<TaskTypeWrapper>();
        }

        #region Interface Implimentation
        public async override Task LoadAsync(int id)
        {
            Id = id;
            await InitializeTaskTypes();
        }

        public override void SetUpItemPropertyChanges()
        {

        }

        public override void RaiseErrorIfItemCreated()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region UI property
        public TaskTypeWrapper SelectedTaskType
        {
            get { return _selectedTaskType; }
            set { _selectedTaskType = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<TaskTypeWrapper> TaskTypes { get; private set; }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand OpenSingleDetailCommand { get; }
        #endregion

        #region Icommand Implimentation
        protected override bool OnSaveDetailCanExecute()
        {
            bool taskTypeStatus = TaskTypes.All(t => !t.HasErrors);
            return taskTypeStatus && HasChanges;
        }

        protected async override void ExecuteOnSaveDetail()
        {
            try
            {
                await _taskTypeRepository.SaveAsync();
                HasChanges = _taskTypeRepository.HasChanges();
                RaiseCollectionSavedEvent();
            }
            catch (Exception exception)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
                MessageDialogServices.ShowInfoDialog("Error while saveing the entities, " +
                    "the data will be reloaded. Detail: " + exception.Message);
                await LoadAsync(Id);
            }
        }

        protected override void ExecuteOnDeleteDetail()
        {
            throw new System.NotImplementedException();
        }

        private void ExecuteOnAddTaskType()
        {
            TaskTypeWrapper newTaskType = new TaskTypeWrapper(new ToDoTaskType());
            newTaskType.PropertyChanged += Wrapper_PropertyChanged;
            _taskTypeRepository.Add(newTaskType.Model);
            TaskTypes.Add(newTaskType);

            newTaskType.Type = ""; //trigger validation error
        }

        private bool OnRemoveTaskTypeCanExecute()
        {
            return  SelectedTaskType != null;
        }

        private async void ExecuteOnRemoveTaskType()
        {
            bool isInToDoTask = await _taskTypeRepository.IsReferencedByTodoTask(SelectedTaskType.Id);
            if (isInToDoTask)
            {
                MessageDialogServices.ShowInfoDialog($"{SelectedTaskType.Type} can't be removed!");
                return;
            }

            SelectedTaskType.PropertyChanged -= Wrapper_PropertyChanged;
            _taskTypeRepository.Remove(SelectedTaskType.Model);
            TaskTypes.Remove(SelectedTaskType);
            SelectedTaskType = null;
            HasChanges = _taskTypeRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        #endregion

        private async Task InitializeTaskTypes()
        {
            foreach (TaskTypeWrapper taskTypeWrapper in TaskTypes)
            {
                taskTypeWrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
            TaskTypes.Clear();
            IEnumerable<ToDoTaskType> types = await _taskTypeRepository.GetAllAsync();
            foreach (ToDoTaskType type in types)
            {
                TaskTypeWrapper taskTypeWrapper = new TaskTypeWrapper(type);
                taskTypeWrapper.PropertyChanged += Wrapper_PropertyChanged;
                TaskTypes.Add(taskTypeWrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _taskTypeRepository.HasChanges();
            }
            if (e.PropertyName == nameof(TaskTypeWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }
}
