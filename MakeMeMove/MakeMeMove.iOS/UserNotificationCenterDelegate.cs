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
            _serviceManager  = new ExerciseServiceManager(_data);
		}

		public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			var exerciseName = response?.Notification?.Request?.Content?.UserInfo[Constants.ExerciseName]?.ToString();
			var exerciseQuantity = int.Parse(response?.Notification?.Request?.Content?.UserInfo[Constants.ExerciseQuantity]?.ToString() ?? "-1");
			
            switch (response?.ActionIdentifier)
            {
                case Constants.CompleteId:
					_data.MarkExerciseNotified(exerciseName, exerciseQuantity);
					_data.MarkExerciseCompleted(exerciseName, exerciseQuantity);
					_serviceManager.RestartNotificationService();
					break;
                case Constants.NextId:
                    _serviceManager.AddInstantExerciseNotificationAndRestartService(exerciseName, exerciseQuantity);
					break;
			}
            
			completionHandler();
		}
	}
}
