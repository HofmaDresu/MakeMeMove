using Android.App;
using Android.Content;
using Android.OS;
using MakeMeMove.Droid.Activities;

namespace MakeMeMove.Droid.ExerciseActions
{
    [Activity(TaskAffinity = "", ExcludeFromRecents = true)]
    public class CompletedActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var exerciseName = Intent.GetStringExtra(Constants.ExerciseName);
            var exerciseQuantity = Intent.GetIntExtra(Constants.ExerciseQuantity, -1);

            if (!string.IsNullOrEmpty(exerciseName) && exerciseQuantity > 0)
            {
                var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager?.CancelAll();
                Data.MarkExerciseCompleted(exerciseName, exerciseQuantity);
            }

            Finish();
        }
    }
}