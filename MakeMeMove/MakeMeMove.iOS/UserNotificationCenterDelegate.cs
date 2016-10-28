using System;
using System.IO;
using SQLite;
using UserNotifications;

namespace MakeMeMove.iOS
{
	public class UserNotificationCenterDelegate: UNUserNotificationCenterDelegate
	{
		private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));
		private readonly ExerciseServiceManager _serviceManager;

		public UserNotificationCenterDelegate()
		{
		}

		public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			var exerciseName = response?.Notification?.Request?.Content?.UserInfo[Constants.ExerciseName]?.ToString();
			var exerciseQuantity = int.Parse(response?.Notification?.Request?.Content?.UserInfo[Constants.ExerciseQuantity]?.ToString() ?? "-1");
			
			// Take action based on Action ID
			switch (response.Notification.Request.Identifier)
			{
				case Constants.NextId:
					_data.MarkExerciseNotified(exerciseName, exerciseQuantity);
					_data.MarkExerciseCompleted(exerciseName, exerciseQuantity);
					_serviceManager.RestartNotificationService();
					break;
				case Constants.CompleteId:
					_serviceManager.AddInstantExerciseNotificationAndRestartService(exerciseName, exerciseQuantity);
					break;
				default:
					// Take action based on identifier
					switch (response.ActionIdentifier)
					{
					}
					break;
			}

			// Inform caller it has been handled
			completionHandler();
		}
	}
}
