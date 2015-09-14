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
