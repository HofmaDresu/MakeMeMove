using System.ComponentModel;
using System.Runtime.CompilerServices;
using MakeMeMove.Annotations;

namespace MakeMeMove.ViewModel
{
    public class EditScheduleViewModel : INotifyPropertyChanged
    {

        private int _schedulePeriodIndex;

        public int SchedulePeriodIndex
        {
            get
            {
                return _schedulePeriodIndex;
            }
            set
            {
                _schedulePeriodIndex = value;
                OnPropertyChanged(nameof(SchedulePeriodIndex));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
