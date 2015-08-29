using System;
using System.Runtime;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MakeMeMove.Model;
using Newtonsoft.Json;


namespace MakeMeMove.Droid
{
    [Service]
    public class ExerciseTickService : Service
    {
        private ExerciseSchedule _exerciseSchedule;
        private Timer _timer;
        private int _pollPeriod = 1 * 60 * 1000;
        //private int _pollPeriod = 10 * 1000;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _timer = new Timer(OnTimerTick, null, _pollPeriod, Timeout.Infinite);

            var settings = GetSharedPreferences("MakeMeMove", FileCreationMode.Private);
            var exercises = settings.GetString("Exercises", string.Empty);
            _exerciseSchedule = JsonConvert.DeserializeObject<ExerciseSchedule>(exercises);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _timer.Dispose();
            base.OnDestroy();
        }

        private void OnTimerTick(object stateInfo)
        {
            var now = DateTime.Now;
            if (now.TimeOfDay > _exerciseSchedule.StartTime.TimeOfDay && now.TimeOfDay < _exerciseSchedule.EndTime.TimeOfDay)
            {
                switch (_exerciseSchedule.Period)
                {
                    case SchedulePeriod.HalfHourly:
                        if(now.Minute == 30) FireNextExerciseNotification();
                        break;
                    case SchedulePeriod.Hourly:
                        if (now.Minute == 0) FireNextExerciseNotification();
                        break;
                    case SchedulePeriod.BiHourly:
                        throw new NotImplementedException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }


            
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
                .SetVisibility(NotificationVisibility.Public);
            }

            
            var notification = builder.Build();
            
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);

        }
    }
}