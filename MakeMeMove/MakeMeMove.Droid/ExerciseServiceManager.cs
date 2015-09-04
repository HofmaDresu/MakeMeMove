﻿using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using MakeMeMove.Droid;
using MakeMeMove.Model;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.Droid
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule)
        {
            var context = Forms.Context;

            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));
            reminder.PutExtra("ExerciseSchedule", JsonConvert.SerializeObject(schedule));

            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            var nextRunTime = TickUtility.GetNextRunTime(schedule);
            Log.Error("asdf", nextRunTime.ToShortDateString() + " " + nextRunTime.ToShortTimeString());
            var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if ((int)Build.VERSION.SdkInt >= 19)
            {
                alarms.SetWindow(AlarmType.RtcWakeup,
                    (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds,
                    10 * 60 * 1000, recurringReminders);
            }
            else
            {
                alarms.Set(AlarmType.RtcWakeup,
                    (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, recurringReminders);
            }

            Toast.MakeText(context, "Service Started", ToastLength.Long).Show();
        }

        public void StopNotificationService(ExerciseSchedule schedule)
        {
            var context = Forms.Context;

            var reminder = new Intent(context, typeof(ExerciseTickBroadcastReceiver));
            var recurringReminders = PendingIntent.GetBroadcast(context, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)context.GetSystemService(Context.AlarmService);

            alarms.Cancel(recurringReminders);

            Toast.MakeText(context, "Service Stopped", ToastLength.Long).Show();

        }
    }
}
