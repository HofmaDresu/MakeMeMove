using MakeMeMove.Model;

namespace MakeMeMove
{
    public interface IServiceManager
    {
        void StartNotificationService(ExerciseSchedule schedule, bool showMessage = true);
        void StopNotificationService(ExerciseSchedule schedule, bool showMessage = true);
        void RestartNotificationServiceIfNeeded(ExerciseSchedule schedule);
        bool NotificationServiceIsRunning();
    }
}
