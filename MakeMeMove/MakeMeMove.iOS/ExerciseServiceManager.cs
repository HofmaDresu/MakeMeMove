using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using MakeMeMove.iOS;
using MakeMeMove.Model;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.iOS
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule)
        {
            var index = new Random().Next(0, schedule.Exercises.Count - 1);

            var nextExercise = schedule.Exercises[index];
            UILocalNotification notification = new UILocalNotification();
            
            notification.AlertAction = "Time to Move";
            notification.AlertBody = $"It's time to do {nextExercise.Quantity} {nextExercise.Name}";
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(15);
            notification.SoundName = UILocalNotification.DefaultSoundName;
            UIApplication.SharedApplication.ScheduleLocalNotification(notification);
        }

        public void StopNotificationService(ExerciseSchedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}
