using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using MakeMeMove.iOS;
using MakeMeMove.Model;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.iOS
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule)
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();

            var now = DateTime.Now;
            var tomorrow = now.AddDays(1);
            var random = new Random();

            for (var testDate = now; testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate).AddMinutes(1))
            {
                var index = random.Next(0, schedule.Exercises.Count - 1);
                
                var nextExercise = schedule.Exercises[index];
                var nextRunTime = TickUtility.GetNextRunTime(schedule, testDate);
                
                var notification = new UILocalNotification
                {
                    AlertAction = "Time to Move",
                    AlertBody = $"It's time to do {nextExercise.Quantity} {nextExercise.Name}",
                    FireDate = nextRunTime.ToNSDate(),
                    SoundName = UILocalNotification.DefaultSoundName,
                    TimeZone = NSTimeZone.LocalTimeZone,
                    RepeatInterval = NSCalendarUnit.Day,
                    ApplicationIconBadgeNumber = -1
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }


            
            
        }

        public void StopNotificationService(ExerciseSchedule schedule)
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();
        }

        public bool NotificationServiceIsRunning()
        {
            return UIApplication.SharedApplication.ScheduledLocalNotifications?.Length > 0;
        }
    }
}
