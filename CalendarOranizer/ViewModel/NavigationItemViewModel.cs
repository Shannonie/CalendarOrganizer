using CalendarOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Windows.Input;

namespace CalendarOrganizer.UI.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;
        private string _detailViewModelName;
        private readonly IEventAggregator _eventAggregator;

        public NavigationItemViewModel(int id, string displayMember,
            string detailViewModelName, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _detailViewModelName = detailViewModelName;
            Id = id;
            DisplayMember = displayMember;

            OpenDetailViewCommand = new DelegateCommand(ExercuteOnOpenDetailView);
        }

        #region UI property
        public int Id { get; }
        public string DisplayMember
        {
            get { return _displayMember; }
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenDetailViewCommand { get; }
        #endregion

        #region Icommand Implimentation
        private void ExercuteOnOpenDetailView()
        {
            _eventAggregator.GetEvent<OpenDetailEvent>().Publish(new OpenDetailEventArgs
            {
                Id = Id,
                ViewModelName = _detailViewModelName
            });
        }
        #endregion
    }
}
