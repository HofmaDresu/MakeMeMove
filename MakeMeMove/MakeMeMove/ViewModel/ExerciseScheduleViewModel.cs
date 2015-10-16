using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MakeMeMove.Model;

namespace MakeMeMove.ViewModel
{
    public class ExerciseScheduleViewModel : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;

        private ExerciseSchedule _exerciseSchedule;
        
        public ExerciseSchedule Schedule
        {
            get
            {
                return _exerciseSchedule;
            }
            set
            {
                _exerciseSchedule = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Schedule"));
            }
        }

        public void NotifyExercisesChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedExercises"));
        }

        public ObservableCollection<ExerciseBlock> SelectedExercises => Schedule.Exercises;
    }
}
