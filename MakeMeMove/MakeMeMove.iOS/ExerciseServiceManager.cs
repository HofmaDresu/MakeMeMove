using System;
using System.Diagnostics;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseServiceManager
	{
		private readonly Data _data;
		public ExerciseServiceManager(Data data)
		{
			_data = data;	
		}

		public void StartNotificationService(bool showMessage = true)
		{
		    StopNotificationService(false);
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
			LocalNotifications.CreateNotification(DateTime.Now, nextExercise);
			ScheduleNotifications();
		}

		void ScheduleNotifications()
		{
			var schedule = _data.GetExerciseSchedule();

			var now = DateTime.Now;
			var tomorrow = now.AddDays(1);

			for (var testDate = TickUtility.GetNextRunTime(schedule, now); testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate.AddMinutes(1)))
			{
				//TODO: figure out how to make this more random. Right now it makes a random schedule, but it's the same every day
				var nextExercise = _data.GetNextEnabledExercise();
				LocalNotifications.CreateNotification(testDate, nextExercise, true);
			}
		}
	}
}
