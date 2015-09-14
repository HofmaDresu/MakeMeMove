using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.Annotations;
using MakeMeMove.Model;

namespace MakeMeMove.ViewModel
{
    public class EditScheduleViewModel : INotifyPropertyChanged
    {

       // public event PropertyChangedEventHandler PropertyChanged;

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
