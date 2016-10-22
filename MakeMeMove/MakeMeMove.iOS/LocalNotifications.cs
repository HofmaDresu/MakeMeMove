using System;
using Foundation;
using MakeMeMove.Model;
using UIKit;
using MakeMeMove.iOS.ExtensionMethods;
using UserNotifications;

namespace MakeMeMove.iOS
{
	public static class LocalNotifications
	{

		public static void CreateNotification(DateTime notificationDate, ExerciseBlock nextExercise, bool isRecurring = false)
		{
			if (nextExercise == null) return;

			var notificationDictionary = new NSMutableDictionary();
			notificationDictionary.Add(new NSString(Constants.ExerciseName), new NSString(nextExercise.CombinedName));
			notificationDictionary.Add(new NSString(Constants.ExerciseQuantity), new NSString(nextExercise.Quantity.ToString()));

			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
				{
					var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
					if (!alertsAllowed) return;

					var content = new UNMutableNotificationContent();
					content.Title = "Time to Move";
					content.Body = $"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}";
					content.Badge = -1;
					content.CategoryIdentifier = Constants.ExerciseNotificationCategoryId;
					content.UserInfo = notificationDictionary;

					var dateComponants = new NSDateComponents
					{
						Hour = notificationDate.Hour,
						Minute = notificationDate.Minute
					};

					var trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponants, true);

					var requestID = $"exerciseNotification_{dateComponants.Hour}:{dateComponants.Minute}";
					var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

					UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
					{
						if (err != null)
						{
							// Do something with error...
						}
					});

				});
			}
			else
			{
				var notification = new UILocalNotification
				{
					AlertAction = "Time to Move",
					AlertBody = $"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}",
					FireDate = notificationDate.ToNSDate(),
					SoundName = UILocalNotification.DefaultSoundName,
					TimeZone = NSTimeZone.LocalTimeZone,
					ApplicationIconBadgeNumber = -1,
					Category = Constants.ExerciseNotificationCategoryId,
					UserInfo = notificationDictionary
				};

				if (isRecurring)
				{
					notification.RepeatInterval = NSCalendarUnit.Day;
				}

				UIApplication.SharedApplication.ScheduleLocalNotification(notification);
			}
		}
	}
}

