using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.Model;

namespace MakeMeMove
{
    public interface ISchedulePersistence
    {
        void SaveExerciseSchedule(ExerciseSchedule schedule);
        ExerciseSchedule LoadExerciseSchedule();
        bool HasExerciseSchedule();
    }
}
