using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using Newtonsoft.Json;
using static System.Math;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    public class ExerciseTickBroadcastReceiver : BroadcastReceiver
    {
        private readonly ISchedulePersistence _schedulePersistence = new SchedulePersistence();

        public override void OnReceive(Context context, Intent intent)
        {
            var preferences = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            if(!preferences.GetBoolean(Constants.ServiceIsStartedKey, false)) return;

            var exerciseSchedule = _schedulePersistence.LoadExerciseSchedule();
            var now = DateTime.Now.TimeOfDay;
            if (now > exerciseSchedule.StartTime.TimeOfDay && now < exerciseSchedule.EndTime.TimeOfDay)
            {
                CreateNotification(context, exerciseSchedule);
            }

            ExerciseServiceManager.SetNextAlarm(context, exerciseSchedule);
        }

        private static void CreateNotification(Context context, ExerciseSchedule exerciseSchedule)
        {
            var index = new Random().Next(0, exerciseSchedule.Exercises.Count);

            var nextExercise = exerciseSchedule.Exercises[Min(index, exerciseSchedule.Exercises.Count - 1)];

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.Name}")
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
            else
            {
                builder
                    .SetSmallIcon(Resource.Drawable.icon);
            }

            var notification = builder.Build();

            const int notificationId = 0;
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager?.Notify(notificationId, notification);
        }
    }
}