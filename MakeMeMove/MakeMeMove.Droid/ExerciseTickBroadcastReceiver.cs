using System;
using Android.App;
using Android.Content;
using Android.OS;
using MakeMeMove.Model;
using Newtonsoft.Json;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    public class ExerciseTickBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var exerciseSchedule =
                JsonConvert.DeserializeObject<ExerciseSchedule>(intent.GetStringExtra("ExerciseSchedule"));
            
            var index = new Random().Next(0, exerciseSchedule.Exercises.Count - 1);

            var nextExercise = exerciseSchedule.Exercises[index];

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.Name}")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Drawable.Icon);

            if ((int)Build.VERSION.SdkInt >= 21)
            {
                builder.SetPriority((int)NotificationPriority.High)
                .SetVisibility(NotificationVisibility.Public)
                .AddAction(0, "Completed", null)
                .AddAction(0, "Skipped", null);
            }

            var notification = builder.Build();

            const int notificationId = 0;
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(notificationId, notification);
        }
    }
}