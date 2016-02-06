using Android.App;
using Android.Content;
using Android.OS;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.DeviceSpecificImplementations;

namespace MakeMeMove.Droid.ExerciseActions
{
    [Activity(TaskAffinity = "", ExcludeFromRecents = true)]
    public class NextActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.CancelAll();

            var exerciseName = Intent.GetStringExtra(Constants.ExerciseName);
            var exerciseQuantity = Intent.GetIntExtra(Constants.ExerciseQuantity, -1);

            if (!string.IsNullOrEmpty(exerciseName) && exerciseQuantity > 0)
            {
                Data.MarkExerciseNotified(exerciseName, -1 * exerciseQuantity);
            }

            UserNotification.CreateNotification(Data, this);

            Finish();
        }
    }
}