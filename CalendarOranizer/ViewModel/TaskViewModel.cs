using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CalendarOrganizer.Model;
using CalendarOrganizer.UI.Data.Lookups;
using CalendarOrganizer.UI.Data.Repositories;
using CalendarOrganizer.UI.Event;
using CalendarOrganizer.UI.View.Services;
using CalendarOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace CalendarOrganizer.UI.ViewModel
{
    /// <summary>
    /// Implimentation of updating selected task from:
    ///     1.DB 
    ///     2.task details change
    /// to MainViewModel
    /// </summary>
    public class TaskViewModel : DetailViewModel, ITaskViewModel
    {
        private ITaskRepository _taskRepository;  //for database:ToDoTasks single access
        private ITaskTypeLookUpService _taskTypeLookUpDataService; //for database:ToDoTaskTypes access
        private TaskWrapper _toDoTask; //for Model:ToDoTask access
        private ContactWrapper _selectedContact; //for Model:Contact access

        public TaskViewModel(
            ITaskRepository taskRepository, ITaskTypeLookUpService taskTypeLookUpDataService,
            IEventAggregator eventAggregator, IMessageDialogServices messageDialogServices)
            : base(eventAggregator, messageDialogServices)
        {
            _taskRepository = taskRepository;
            _taskTypeLookUpDataService = taskTypeLookUpDataService;
            eventAggregator.GetEvent<AfterCollectionSavedEvent>()
                .Subscribe(ExecuteAfterCollectionSaved);

            AddContactCommand = new DelegateCommand(ExecuteOnAddContact);
            RemoveContactCommand = new DelegateCommand(ExecuteOnRemoveContact,
                OnRemoveContactCanExecute);

            ToDoTaskTypes = new ObservableCollection<LookUpItem>();
            Contacts = new ObservableCollection<ContactWrapper>();
        }

        #region Interface Implimentation
        public override async Task LoadAsync(int taskId)
        {
            ToDoTask task = taskId > 0 ?
                await _taskRepository.GetByIDAsync(taskId) : CreateNewTask();
            Id = taskId;

            InitializeTask(task);
            await LoadTaskTypeLookUpAsync();
            InitializeTaskContacts(task.TaskContacts);
        }

        public override void SetUpItemPropertyChanges()
        {
            TaskItem.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _taskRepository.HasChanges();
                }

                if (e.PropertyName == nameof(TaskItem.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
                if (e.PropertyName == nameof(TaskItem.TaskName))
                {
                    SetItemTitle(TaskItem.TaskName);
                }
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public override void RaiseErrorIfItemCreated()
        {
            if (TaskItem.Id == 0) // for new created TaskItem, trigger validation
            {
                TaskItem.TaskName = "";
                TaskItem.TaskTime = DateTime.Now;
            }
        }
        #endregion

        #region UI property
        public ContactWrapper SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveContactCommand).RaiseCanExecuteChanged();
            }
        }
        public TaskWrapper TaskItem
        {
            get { return _toDoTask; }
            set
            {
                _toDoTask = value;
                OnPropertyChanged();
            }
        }
        //ObservableCollection<>: derived from INotifyCollectionChanged, notify changes to databinding
        public ObservableCollection<LookUpItem> ToDoTaskTypes { get; }
        public ObservableCollection<ContactWrapper> Contacts { get; }
        public ICommand AddContactCommand { get; }
        public ICommand RemoveContactCommand { get; }
        #endregion

        #region Icommand Implimentation
        protected override bool OnSaveDetailCanExecute()
        {
            bool taskStatus = TaskItem != null && !TaskItem.HasErrors;
            return taskStatus
                && Contacts.All(ct => !ct.HasErrors)
                && HasChanges;
        }

        protected override async void ExecuteOnSaveDetail()
        {
            await ExecuteOnSaveDetailWithOpmisticConcurrencyAsync(
                _taskRepository.SaveAsync,
                () =>
                {
                    HasChanges = _taskRepository.HasChanges();
                    Id = TaskItem.Id;
                    RaiseDetailSavedEvent(TaskItem.Id, $"{TaskItem.TaskName}");
                    RaiseDetailSavedEvent(TaskItem.Id, $"{TaskItem.TaskName}");
                });
        }

        protected override async void ExecuteOnDeleteDetail()
        {
            if (await _taskRepository.IsInDateTodos(TaskItem.Id))
            {
                MessageDialogServices.ShowInfoDialog($"{TaskItem.TaskName} can't be deleted!");
                return;
            }

            MessageDialogOptions response = await MessageDialogServices.ShowOkCancelDialogMessage(
                $"Do you want to delete task {TaskItem.TaskName}?", "Question");

            if (response.Equals(MessageDialogOptions.OK))
            {
                _taskRepository.Remove(TaskItem.Model);
                await _taskRepository.SaveAsync();
                RaiseDetailDeletedEvent(TaskItem.Id);
            }
        }

        private bool OnRemoveContactCanExecute()
        {
            return SelectedContact != null;
        }

        private void ExecuteOnRemoveContact()
        {
            SelectedContact.PropertyChanged -= ContactWrapper_PropertyChanged;
            _taskRepository.RemoveContact(SelectedContact.Model);
            Contacts.Remove(SelectedContact);
            HasChanges = _taskRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void ExecuteOnAddContact()
        {
            ContactWrapper newContact = new ContactWrapper(new Contact());
            newContact.PropertyChanged += ContactWrapper_PropertyChanged;
            Contacts.Add(newContact);
            TaskItem.Model.TaskContacts.Add(newContact.Model);

            newContact.Name = ""; //trigger validation error
            newContact.PhoneNumber = ""; //trigger validation error
        }

        private async void ExecuteAfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(TaskTypeViewModel))
            {
                await LoadTaskTypeLookUpAsync();
            }
        }
        #endregion

        private void InitializeTask(ToDoTask task)
        {
            TaskItem = new TaskWrapper(task);
            SetUpItemPropertyChanges();
            RaiseErrorIfItemCreated();
            SetItemTitle(TaskItem.TaskName);
        }

        private ToDoTask CreateNewTask()
        {
            ToDoTask newTask = new ToDoTask();
            _taskRepository.Add(newTask);
            return newTask;
        }

        private async Task LoadTaskTypeLookUpAsync()
        {
            ToDoTaskTypes.Clear();
            IEnumerable<LookUpItem> lookUpTypes =
                await _taskTypeLookUpDataService.LookUpTaskTypeAsync();
            ToDoTaskTypes.Add(new LookUpNullItem { DisplayItem = " - " });
            foreach (LookUpItem type in lookUpTypes)
            {
                ToDoTaskTypes.Add(type);
            }
        }

        private void InitializeTaskContacts(ICollection<Contact> taskContacts)
        {
            foreach (ContactWrapper wrapper in Contacts)
            {
                wrapper.PropertyChanged -= ContactWrapper_PropertyChanged;
            }
            Contacts.Clear();
            foreach (Contact contact in taskContacts)
            {
                ContactWrapper wrapper = new ContactWrapper(contact);
                Contacts.Add(wrapper);
                wrapper.PropertyChanged += ContactWrapper_PropertyChanged;
            }
        }

        private void ContactWrapper_PropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _taskRepository.HasChanges();
            }
            if (e.PropertyName == nameof(ContactWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }
}
