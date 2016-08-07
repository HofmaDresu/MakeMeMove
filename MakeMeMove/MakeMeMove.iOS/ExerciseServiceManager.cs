using System;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseServiceManager
	{
		private Data _data;
		public ExerciseServiceManager(Data data)
		{
			_data = data;	
		}

		public void StartNotificationService(bool showMessage = true)
		{
			UIApplication.SharedApplication.CancelAllLocalNotifications();


			var schedule = _data.GetExerciseSchedule();
			var exercises = _data.GetExerciseBlocks();

			var now = DateTime.Now;
			var tomorrow = now.AddDays(1);
			var random = new Random();

			for (var testDate = TickUtility.GetNextRunTime(schedule, now); testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate.AddMinutes(1)))
			{
				var index = random.Next(0, exercises.Count);
				//TODO: figure out how to make this more random. Right now it makes a random schedule, but it's the same every day
				var nextExercise = exercises[Math.Min(index, exercises.Count - 1)];
				UserNotification.CreateNotification(testDate, nextExercise, true);
			}
		}

		public void StopNotificationService(bool showMessage = true)
		{
			UIApplication.SharedApplication.CancelAllLocalNotifications();
		}

		public void RestartNotificationServiceIfNeeded()
		{
			if (NotificationServiceIsRunning())
			{
				StopNotificationService(false);
				StartNotificationService(false);
			}
		}

		public bool NotificationServiceIsRunning()
		{
			return UIApplication.SharedApplication.ScheduledLocalNotifications?.Length > 0;
		}
	}
}
