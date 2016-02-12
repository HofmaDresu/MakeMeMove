using System;
using Android.Content;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using SQLite;
using Environment = System.Environment;
using Path = System.IO.Path;

namespace MakeMeMove.Droid
{
    [BroadcastReceiver]
    public class ExerciseTickBroadcastReceiver : BroadcastReceiver
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));

        public override void OnReceive(Context context, Intent intent)
        {
            var preferences = context.GetSharedPreferences(Constants.SharedPreferencesKey, FileCreationMode.Private);
            if(!preferences.GetBoolean(Constants.ServiceIsStartedKey, false)) return;

            var exerciseSchedule = _data.GetExerciseSchedule();
            var now = DateTime.Now.TimeOfDay;
            if (now > exerciseSchedule.StartTime.TimeOfDay && now < exerciseSchedule.EndTime.TimeOfDay)
            {
                UserNotification.CreateNotification(_data, context);
            }

            ExerciseServiceManager.SetNextAlarm(context, exerciseSchedule);
        }
    }
}