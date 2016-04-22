using System;
using Android.App;
using Android.Content;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using SQLite;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    public class ChangeExerciseBroadcastReceiver : BroadcastReceiver
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));

        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.CancelAll();

            var exerciseName = intent.GetStringExtra(Constants.ExerciseName);
            var exerciseQuantity = intent.GetIntExtra(Constants.ExerciseQuantity, -1);

            if (!string.IsNullOrEmpty(exerciseName) && exerciseQuantity > 0)
            {
                _data.MarkExerciseNotified(exerciseName, -1 * exerciseQuantity);
            }

            UserNotification.CreateNotification(_data, context);
        }
    }
}