using System;
using System.IO;
using Android.App;
using Android.Content;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using SQLite;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.LowPriority)]
    public class ExerciseRebootReceiver : BroadcastReceiver
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));

        public override void OnReceive(Context context, Intent intent)
        {
            var preferences = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            if (!preferences.GetBoolean(Constants.ServiceIsStartedKey, false)) return;

            var exerciseSchedule = _data.GetExerciseSchedule();
            ExerciseServiceManager.SetNextAlarm(context, exerciseSchedule);
        }
    }
}