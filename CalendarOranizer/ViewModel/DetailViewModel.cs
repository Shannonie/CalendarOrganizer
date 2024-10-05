using CalendarOrganizer.UI.Event;
using CalendarOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarOrganizer.UI.ViewModel
{
    public abstract class DetailViewModel : ViewModelBase, IDetailViewModel
    {
        private int _id;
        private string _title;
        private bool _hasChanges;
        protected readonly IEventAggregator EventAggregator; //for viewModel communication
        protected IMessageDialogServices MessageDialogServices;

        public DetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogServices messageDialogServices)
        {
            EventAggregator = eventAggregator;
            MessageDialogServices = messageDialogServices;

            SaveCommand = new DelegateCommand(ExecuteOnSaveDetail, OnSaveDetailCanExecute);
            DeleteCommand = new DelegateCommand(ExecuteOnDeleteDetail);
            CloseDetailTabCommand = new DelegateCommand(ExecuteOnCloseDetailTabDetail);
        }

        #region Interface implimentation
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public string Title
        {
            get { return _title; }
            protected set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public void SetItemTitle(string title)
        {
            Title = title;
        }

        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public abstract Task LoadAsync(int id);

        public abstract void SetUpItemPropertyChanges();

        public abstract void RaiseErrorIfItemCreated();
        #endregion

        #region UI property
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand CloseDetailTabCommand { get; }
        #endregion

        #region Icommand Implimentation
        protected abstract void ExecuteOnSaveDetail();
        protected abstract bool OnSaveDetailCanExecute();
        protected abstract void ExecuteOnDeleteDetail();
        protected virtual async void ExecuteOnCloseDetailTabDetail()
        {
            if (HasChanges)
            {
                if (await MessageDialogServices.ShowOkCancelDialogMessage(
                    "You've made changes, close this detail?",
                    "Question") == MessageDialogOptions.Cancel)
                {
                    return;
                }
            }
            EventAggregator.GetEvent<AfterDetailClosedTabEvent>().Publish(
                new AfterDetailDeletedEventArgs()
                {
                    Id = this.Id,
                    ViewModelName = this.GetType().Name
                });
        }
        #endregion

        protected virtual void RaiseCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>().Publish(
                new AfterCollectionSavedEventArgs
                {
                    ViewModelName = this.GetType().Name
                });
        }

        protected virtual void RaiseDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name,
                    DisplayMember = displayMember
                });
        }

        protected virtual void RaiseDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(
                new AfterDetailDeletedEventArgs
                {
                    Id = modelId,
                    ViewModelName = this.GetType().Name
                });
        }

        protected async Task ExecuteOnSaveDetailWithOpmisticConcurrencyAsync(
            Func<Task> saveAsyncFunc, Action DoAfterSaveAction)
        {
            try
            {
                await saveAsyncFunc();
            }
            catch (DbUpdateConcurrencyException exeption)
            {
                DbEntityEntry entry = exeption.Entries.Single();
                CheckIfEntityExist(entry);
                await CheckIfEntityMotified(saveAsyncFunc, entry);
            }

            DoAfterSaveAction();
        }

        private async Task CheckIfEntityMotified(
            Func<Task> saveAsyncFunc, DbEntityEntry entry)
        {
            MessageDialogOptions response = await MessageDialogServices.ShowOkCancelDialogMessage(
                "The entity has been changed in database. Click 'OK' to save your changes " +
                "anyways, otherwise reload from the database", "Question");
            if (response == MessageDialogOptions.OK)
            {// update the original values with database-values
                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                await saveAsyncFunc(); ;
            }
            else
            {//
                await entry.ReloadAsync();
                await LoadAsync(Id);
            }
        }

        private void CheckIfEntityExist(DbEntityEntry entry)
        {
            DbPropertyValues dbEntityValue = entry.GetDatabaseValues();
            if (dbEntityValue == null)
            {
                MessageDialogServices.ShowInfoDialog(
                    "The entity has been deleteded in database.");
                RaiseDetailDeletedEvent(Id);
                return;
            }
        }
    }
}
