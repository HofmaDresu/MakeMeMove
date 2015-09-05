using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.LowPriority)]
    public class ExerciseRebootReceiver : BroadcastReceiver
    {
        private readonly ISchedulePersistence _schedulePersistence = new SchedulePersistence();

        public override void OnReceive(Context context, Intent intent)
        {
            var preferences = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            if (!preferences.GetBoolean(Constants.ServiceIsStartedKey, false)) return;

            var exerciseSchedule = _schedulePersistence.LoadExerciseSchedule();
            ExerciseTickBroadcastReceiver.SetNextAlarm(context, exerciseSchedule);
        }
    }
}