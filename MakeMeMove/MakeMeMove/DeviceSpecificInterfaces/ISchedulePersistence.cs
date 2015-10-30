using MakeMeMove.Model;

namespace MakeMeMove.DeviceSpecificInterfaces
{
    public interface ISchedulePersistence
    {
        void SaveExerciseSchedule(ExerciseSchedule schedule);
        ExerciseSchedule LoadExerciseSchedule();
        bool HasExerciseSchedule();
        void RemoveAllData();
    }
}
