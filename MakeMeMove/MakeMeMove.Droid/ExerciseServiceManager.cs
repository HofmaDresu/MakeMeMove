using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using MakeMeMove.Droid;
using MakeMeMove.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.Droid
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule, bool showMessage = true)
        {
            var context = Forms.Context;

            SetNextAlarm(context, schedule);

            SaveServiceStatus(context, true);

            if(showMessage) Toast.MakeText(context, "Service Started", ToastLength.Long).Show();
        }

        public void StopNotificationService(ExerciseSchedule schedule, bool showMessage = true)
        {
            var context = Forms.Context;

            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));
            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            alarms.Cancel(recurringReminders);

            SaveServiceStatus(context, false);

            if (showMessage) Toast.MakeText(context, "Service Stopped", ToastLength.Long).Show();

        }

        public void RestartNotificationServiceIfNeeded(ExerciseSchedule schedule)
        {
            if (NotificationServiceIsRunning())
            {
                StopNotificationService(schedule, false);
                StartNotificationService(schedule, false);
            }
        }

        public bool NotificationServiceIsRunning()
        {
            var preferences = Forms.Context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            return preferences.GetBoolean(Constants.ServiceIsStartedKey, false);
        }

        private void SaveServiceStatus(Context context, bool serviceIsStarted)
        {
            var editor = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private).Edit();
            editor.PutBoolean(Constants.ServiceIsStartedKey, serviceIsStarted);
            editor.Commit();
        }

        public static void SetNextAlarm(Context context, ExerciseSchedule exerciseSchedule)
        {
            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));

            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var nextRunTime = TickUtility.GetNextRunTime(exerciseSchedule);

            var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                
            if ((int)Build.VERSION.SdkInt >= 23)
            {

                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, Constants.NotificationPreferences);
                var allowWakeFromIdle = JsonConvert.DeserializeObject<bool>(File.ReadAllText(filePath));

                Log.Error("asdf", $"Allows wake from idle: {allowWakeFromIdle}");

                if (allowWakeFromIdle)
                {
                    alarms.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup,
                        (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds,
                        recurringReminders);
                }
                else
                {
                    alarms.SetExact(AlarmType.RtcWakeup,
                        (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds,
                        recurringReminders);
                }
            }
            else if ((int) Build.VERSION.SdkInt >= 19)
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
