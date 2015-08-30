using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using MakeMeMove.Model;
using Newtonsoft.Json;

namespace MakeMeMove.Droid
{
    [Service]
    public class ExerciseTickService : Service
    {
        private ExerciseSchedule _exerciseSchedule;
        private Timer _timer;
        private int _pollPeriod;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var settings = GetSharedPreferences("MakeMeMove", FileCreationMode.Private);
            var exercises = settings.GetString("Exercises", string.Empty);
            _exerciseSchedule = JsonConvert.DeserializeObject<ExerciseSchedule>(exercises);

            _pollPeriod = TickUtility.GetPollPeriod(_exerciseSchedule.Period);

            var startMilliseconds = TickUtility.GetFirstRunTimeSpan(_exerciseSchedule);

            _timer = new Timer(OnTimerTick, null, startMilliseconds, Timeout.Infinite);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _timer.Dispose();
            base.OnDestroy();
        }

        private void OnTimerTick(object stateInfo)
        {
            FireNextExerciseNotification();
            _timer.Change(_pollPeriod, Timeout.Infinite);
        }

        private void FireNextExerciseNotification()
        {
            var index = new Random().Next(0, _exerciseSchedule.Exercises.Count - 1);

            var nextExercise = _exerciseSchedule.Exercises[index];
            
            var builder = new Notification.Builder(this)
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
            
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);

        }
    }
}