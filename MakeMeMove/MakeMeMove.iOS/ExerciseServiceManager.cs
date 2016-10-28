﻿using System;
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
            ScheduleNotifications();
            _data.SetIosServiceRunningStatus(true);
		}

		public void StopNotificationService(bool showMessage = true)
		{
			UIApplication.SharedApplication.CancelAllLocalNotifications();
            _data.SetIosServiceRunningStatus(false);
        }

		public void RestartNotificationServiceIfNeeded()
		{
			if (NotificationServiceIsRunning())
			{
			    RestartNotificationService();
			}
		}

	    public void RestartNotificationService()
	    {
	        StopNotificationService(false);
	        StartNotificationService(false);
	    }

	    public bool NotificationServiceIsRunning()
		{
	        return _data.IsIosServiceRunning();
		}

		public void AddInstantExerciseNotificationAndRestartService(string exerciseName, int exerciseQuantity)
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
			LocalNotifications.CreateInstantNotification(nextExercise);
			ScheduleNotifications();
		}

		void ScheduleNotifications()
		{
			var schedule = _data.GetExerciseSchedule();

			var now = DateTime.Now;
			var tomorrow = now.AddDays(1);
		    ExerciseBlock exercise = null;
		    var random = new Random();

			for (var testDate = TickUtility.GetNextRunTime(schedule, now); testDate < tomorrow; testDate = TickUtility.GetNextRunTime(schedule, testDate.AddMinutes(1)))
			{
				//TODO: figure out how to make this more random. Right now it makes a random schedule, but it's the same every day
				exercise = exercise == null ? _data.GetNextEnabledExercise(random) : _data.GetNextDifferentEnabledExercise(exercise, random);
				LocalNotifications.CreateNotification(testDate, exercise, true);
			}
		}
	}
}
