using System;
using MakeMeMove.Model;
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
			ScheduleNotifications();
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

		public void AddInstantExerciseNotification(string exerciseName, int exerciseQuantity)
		{
			if (!string.IsNullOrEmpty(exerciseName) && exerciseQuantity > 0)
			{
				_data.MarkExerciseNotified(exerciseName, -1 * exerciseQuantity);
			}

			var nextExercise =
				_data.GetNextDifferentEnabledExercise(new ExerciseBlock
				{
					Name = exerciseName,
					Quantity = exerciseQuantity
				});

			UIApplication.SharedApplication.CancelAllLocalNotifications();
			UserNotification.CreateNotification(DateTime.Now, nextExercise);
			ScheduleNotifications();
		}

		void ScheduleNotifications()
		{
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
	}
}
