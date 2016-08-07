using System;
using Foundation;
using MakeMeMove.Model;
using UIKit;
using MakeMeMove.iOS.ExtensionMethods;

namespace MakeMeMove.iOS
{
	public static class UserNotification
	{

		public static void CreateNotification(DateTime notificationDate, ExerciseBlock nextExercise, bool isRecurring = false)
		{
			var notificationDictionary = new NSMutableDictionary();
			notificationDictionary.Add(new NSString(Constants.ExerciseName), new NSString(nextExercise.Name));
			notificationDictionary.Add(new NSString(Constants.ExerciseQuantity), new NSString(nextExercise.Quantity.ToString()));

			var notification = new UILocalNotification
			{
				AlertAction = "Time to Move",
				AlertBody = $"It's time to do {nextExercise.Quantity} {nextExercise.Name}",
				FireDate = notificationDate.ToNSDate(),
				SoundName = UILocalNotification.DefaultSoundName,
				TimeZone = NSTimeZone.LocalTimeZone,
				ApplicationIconBadgeNumber = -1,
				Category = Constants.UnregisteredExerciseCategoryId,
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

