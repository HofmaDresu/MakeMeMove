using System;
using Android.App;
using Android.Content;
using Android.OS;
using MakeMeMove.Model;

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

            SetNextAlarm(context, exerciseSchedule);
        }

        private static void CreateNotification(Context context, ExerciseSchedule exerciseSchedule)
        {
            var index = new Random().Next(0, exerciseSchedule.Exercises.Count - 1);

            var nextExercise = exerciseSchedule.Exercises[index];

            var builder = new Notification.Builder(context)
                .SetContentTitle("Time to Move")
                .SetContentText($"It's time to do {nextExercise.Quantity} {nextExercise.Name}")
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetSmallIcon(Resource.Drawable.icon);

            if ((int) Build.VERSION.SdkInt >= 21)
            {
                builder
                    .SetPriority((int)NotificationPriority.High)
                    .SetVisibility(NotificationVisibility.Public)
                    .SetCategory("reminder");
            }

            var notification = builder.Build();

            const int notificationId = 0;
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(notificationId, notification);
        }

        public static void SetNextAlarm(Context context, ExerciseSchedule exerciseSchedule)
        {
            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));

            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var nextRunTime = TickUtility.GetNextRunTime(exerciseSchedule);

            var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if ((int) Build.VERSION.SdkInt >= 19)
            {
                alarms.SetExact(AlarmType.RtcWakeup,
                    (long) nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds,
                    recurringReminders);
            }
            else
            {
                alarms.Set(AlarmType.RtcWakeup,
                    (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, 
                    recurringReminders);
            }
        }
    }
}