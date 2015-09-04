using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
                _exerciseSchedule = _exerciseSchedule ?? ExerciseSchedule.CreateDefaultSchedule();

                return _exerciseSchedule;
            }
            set
            {
                _exerciseSchedule = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExerciseSchedule"));
            }
        }

        public List<ExerciseBlock> SelectedExercises => Schedule.Exercises;
    }
}
