using System;
using System.Collections.Generic;
using Foundation;
using MakeMeMove.iOS.ExtensionMethods;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseServiceManager
	{
		public void StartNotificationService(ExerciseSchedule schedule, List<ExerciseBlock> exercises, bool showMessage = true)
		{
			UIApplication.SharedApplication.CancelAllLocalNotifications();

			var now = DateTime.Now;
			var tomorrow = now.AddDays(1);
			var random = new Random();

			for (var testDate = TickUtility.GetNextRunTime(schedule, now); testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate.AddMinutes(1)))
			{
				var index = random.Next(0, exercises.Count);
				//TODO: figure out how to make this more random. Right now it makes a random schedule, but it's the same every day
				var nextExercise = exercises[Math.Min(index, exercises.Count - 1)];

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

		public void RestartNotificationServiceIfNeeded(ExerciseSchedule schedule, List<ExerciseBlock> exercises)
		{
			if (NotificationServiceIsRunning())
			{
				StopNotificationService(schedule, false);
				StartNotificationService(schedule, exercises, false);
			}
		}

		public bool NotificationServiceIsRunning()
		{
			return UIApplication.SharedApplication.ScheduledLocalNotifications?.Length > 0;
		}
	}
}
