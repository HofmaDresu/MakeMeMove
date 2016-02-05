using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Droid.ExerciseActions;
using MakeMeMove.Model;
using Newtonsoft.Json;
using SQLite;
using static System.Math;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    public class ExerciseTickBroadcastReceiver : BroadcastReceiver
    {
        private readonly Data _data = new Data(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));

        public override void OnReceive(Context context, Intent intent)
        {
            var preferences = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            if(!preferences.GetBoolean(Constants.ServiceIsStartedKey, false)) return;

            var exerciseSchedule = _data.GetExerciseSchedule();
            var now = DateTime.Now.TimeOfDay;
            if (now > exerciseSchedule.StartTime.TimeOfDay && now < exerciseSchedule.EndTime.TimeOfDay)
            {
                CreateNotification(context, _data.GetExerciseBlocks());
            }

            ExerciseServiceManager.SetNextAlarm(context, exerciseSchedule);
        }

        private static void CreateNotification(Context context, List<ExerciseBlock> exercises)
        {
            var enabledExercises = exercises.Where(e => e.Enabled).ToList();

            if (enabledExercises.Count == 0) return;

            var index = new Random().Next(0, enabledExercises.Count);

            var nextExercise = enabledExercises[Min(index, exercises.Count - 1)];

            var completedIntent = new Intent(context, typeof (CompletedActivity));
            completedIntent.PutExtra(Constants.ExerciseName, nextExercise.CombinedName);
            completedIntent.PutExtra(Constants.ExerciseQuantity, nextExercise.Quantity);
            var pendingIntent = PendingIntent.GetActivity(context, DateTime.Now.Millisecond, completedIntent, 0);

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate);
            
            if ((int) Build.VERSION.SdkInt >= 21)
            {
                builder
                    .SetPriority((int)NotificationPriority.High)
                    .SetVisibility(NotificationVisibility.Public)
                    .SetCategory("reminder")
                    .SetSmallIcon(Resource.Drawable.Mmm_white_icon)
                    .SetColor(Color.Rgb(215, 78, 10));
            }
            else if ((int)Build.VERSION.SdkInt >= 20)
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon);
            }
            else
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon);
            }

            var notification = builder.Build();

            notification.Flags |= NotificationFlags.AutoCancel;

            const int notificationId = 0;
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.Notify(notificationId, notification);
        }
    }
}