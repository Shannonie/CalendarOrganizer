using Autofac.Features.Indexed;
using CalendarOrganizer.UI.Event;
using CalendarOrganizer.UI.View.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CalendarOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IDetailViewModel _selectedDetailViewModel;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator; //for ViewModels creation
        private IEventAggregator _eventAggregator; //for viewModel communication
        private IMessageDialogServices _messageDialogServices;

        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogServices messageDialogServices)
        {
            _detailViewModelCreator = detailViewModelCreator;
            _eventAggregator = eventAggregator;
            _messageDialogServices = messageDialogServices;
            NavigationViewModel = navigationViewModel;

            _eventAggregator.GetEvent<OpenDetailEvent>()
                .Subscribe(ExercuteOnOpenDetail);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(ExercuteAfterDeletedDetail);
            _eventAggregator.GetEvent<AfterDetailClosedTabEvent>()
                .Subscribe(ExercuteAfterDetailClosedTab);
            CreateNewDetailCommand = new DelegateCommand<Type>(ExercuteOnCreateNewDetail);
            OpenSingleDetailCommand = new DelegateCommand<Type>(ExercuteOpenSingleDetail);

            DetailViewModels = new ObservableCollection<IDetailViewModel>();
        }

        #region UI property
        public INavigationViewModel NavigationViewModel { get; }
        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }
        public IDetailViewModel SelectedDetailViewModel
        {
            get { return _selectedDetailViewModel; }
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }
        public ICommand CreateNewDetailCommand { get; }
        public ICommand OpenSingleDetailCommand { get; }
        #endregion

        #region Icommand Implimentation
        private void ExercuteOnCreateNewDetail(Type viewModelType)
        {
            ExercuteOnOpenDetail(new OpenDetailEventArgs
            { ViewModelName = viewModelType.Name });
        }

        private async void ExercuteOnOpenDetail(OpenDetailEventArgs args)
        {
            IDetailViewModel detailViewModel = DetailViewModels.SingleOrDefault(
                vm => vm.Id == args.Id && vm.GetType().Name == args.ViewModelName);
            if (detailViewModel == null)
            {
                detailViewModel = _detailViewModelCreator[args.ViewModelName];

                try
                {
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch
                {
                    _messageDialogServices.ShowInfoDialog("Could not load entity.");
                    await NavigationViewModel.LoadAsync();
                    return;
                }

                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }

        private void ExercuteOpenSingleDetail(Type viewModelType)
        {
            ExercuteOnOpenDetail(new OpenDetailEventArgs
            {
                Id = -1,
                ViewModelName = viewModelType.Name
            });
        }

        private void ExercuteAfterDeletedDetail(AfterDetailDeletedEventArgs args)
        {
            DeleteTaskDetail(args.Id, args.ViewModelName);
        }

        private void ExercuteAfterDetailClosedTab(AfterDetailDeletedEventArgs args)
        {
            DeleteTaskDetail(args.Id, args.ViewModelName);
        }
        #endregion

        private async void NoticeSelectionChange()
        {
            if (SelectedDetailViewModel != null && SelectedDetailViewModel.HasChanges)
            {
                if (await _messageDialogServices.ShowOkCancelDialogMessage(
                    "You've made changes, navigate to another task detail?",
                    "Notice") == MessageDialogOptions.Cancel)
                {
                    return;
                }
            }
        }

        public async Task LoadToDoTasksAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private void DeleteTaskDetail(int? viewModelId, string viewModelName)
        {
            IDetailViewModel detailViewModel = DetailViewModels.SingleOrDefault(
                vm => vm.Id == viewModelId && vm.GetType().Name == viewModelName);
            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
        }
    }
}
