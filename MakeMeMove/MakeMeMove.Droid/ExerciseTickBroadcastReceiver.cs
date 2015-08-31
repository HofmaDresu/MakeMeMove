using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech.Tts;
using Android.Util;
using Java.Lang;
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
                .SetSmallIcon(Resource.Drawable.Icon);

            if ((int) Build.VERSION.SdkInt >= 21)
            {
                builder.SetPriority((int) NotificationPriority.High)
                    .SetVisibility(NotificationVisibility.Public)
                    .AddAction(0, "Completed", null)
                    .AddAction(0, "Skipped", null);
            }

            var notification = builder.Build();

            const int notificationId = 0;
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(notificationId, notification);
        }

        private void SetNextAlarm(Context context, ExerciseSchedule exerciseSchedule)
        {
            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));
            reminder.PutExtra("ExerciseSchedule", JsonConvert.SerializeObject(exerciseSchedule));

            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var nextRunTime = TickUtility.GetNextRunTime(exerciseSchedule);
            Log.Error("asdf", nextRunTime.ToShortDateString() + " " + nextRunTime.ToShortTimeString());
            var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            alarms.SetExact(AlarmType.RtcWakeup, (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, recurringReminders);
        }
    }
}