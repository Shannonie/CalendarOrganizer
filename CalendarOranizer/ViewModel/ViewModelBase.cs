using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace CalendarOrganizer.UI.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            //[CallerMemberName]:Compiler will pass the function name automatically on runtime
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Brush mBarColor { get; set; }
        public Brush BarColor
        {
            set
            {
                if (value != mBarColor)
                {
                    mBarColor = value;
                }

                OnPropertyChanged(nameof(BarColor));
            }
            get
            {
                return mBarColor;
            }
        }
    }
}
