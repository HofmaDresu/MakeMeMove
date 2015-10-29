using System;
using Foundation;
using MakeMeMove.iOS;
using MakeMeMove.Model;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using static System.Math;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.iOS
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule, bool showMessage = true)
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();

            var now = DateTime.Now;
            var tomorrow = now.AddDays(1);
            var random = new Random();

            for (var testDate = TickUtility.GetNextRunTime(schedule, now); testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate.AddMinutes(1)))
            {
                Console.WriteLine("###################################################");
                Console.WriteLine($"Added time:{testDate.ToShortDateString()} {testDate.ToShortTimeString()}");

                Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                var index = random.Next(0, schedule.Exercises.Count);
                //TODO: figure out how to make this more random. Right now it makes a random schedule, but it's the same every day
                var nextExercise = schedule.Exercises[Min(index, schedule.Exercises.Count - 1)];
                
                var notification = new UILocalNotification
                {
                    AlertAction = "Time to Move",
                    AlertBody = $"It's time to do {nextExercise.Quantity} {nextExercise.Name}",
                    FireDate = testDate.ToNSDate(),
                    SoundName = UILocalNotification.DefaultSoundName,
                    TimeZone = NSTimeZone.LocalTimeZone,
                    RepeatInterval = NSCalendarUnit.Day,
                    ApplicationIconBadgeNumber = -1
                };

                UIApplication.SharedApplication.ScheduleLocalNotification(notification);
            }
        }

        public void StopNotificationService(ExerciseSchedule schedule, bool showMessage = true)
        {
            UIApplication.SharedApplication.CancelAllLocalNotifications();
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
            return UIApplication.SharedApplication.ScheduledLocalNotifications?.Length > 0;
        }
    }
}
