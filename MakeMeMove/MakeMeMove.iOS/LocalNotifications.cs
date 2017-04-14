﻿using System;
using Foundation;
using MakeMeMove.Model;
using UIKit;
using MakeMeMove.iOS.ExtensionMethods;
using UserNotifications;
using Humanizer;

namespace MakeMeMove.iOS
{
	public static class LocalNotifications
	{

	    public static void CreateInstantNotification(ExerciseBlock nextExercise)
	    {
	        CreateNotification(DateTime.Now.AddSeconds(3), nextExercise, false, true);
	    }

		public static void CreateNotification(DateTime notificationDate, ExerciseBlock nextExercise, bool isRecurring = false, bool isInstant = false)
		{
			if (nextExercise == null) return;

		    var notificationDictionary = new NSMutableDictionary
		    {
		        {new NSString(Constants.ExerciseName), new NSString(nextExercise.CombinedName)},
		        {new NSString(Constants.ExerciseQuantity), new NSString(nextExercise.Quantity.ToString())}
		    };

            var defaults = NSUserDefaults.StandardUserDefaults;
            var soundString = defaults.StringForKey(Constants.UserDefaultsNotificationSoundsKey);
            var soundEnum = soundString.DehumanizeTo<Constants.NotificationSounds>();

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.GetNotificationSettings(settings =>
				{
					var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
					if (!alertsAllowed) return;

                    var content = new UNMutableNotificationContent
                    {
                        Title = "Time to Move",
                        Body = $"It's time to do {nextExercise.Quantity} {nextExercise.CombinedName}",
                        Badge = -1,
                        CategoryIdentifier = Constants.ExerciseNotificationCategoryId,
                        UserInfo = notificationDictionary,
                        Sound = UNNotificationSound.GetSound(Constants.NotificaitonSoundsMap[soundEnum])
                    };

				    UNNotificationTrigger trigger;
				    string requestId;

				    if (isInstant)
                    {
                        trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(3, false);
                        requestId = "exerciseNotification_Instant";
                    }
				    else
                    {

                        var dateComponants = new NSDateComponents
                        {
                            Hour = notificationDate.Hour,
                            Minute = notificationDate.Minute,
                            Second = notificationDate.Second
                        };
                        trigger = UNCalendarNotificationTrigger.CreateTrigger(dateComponants, true);
                        requestId = $"exerciseNotification_{dateComponants.Hour}:{dateComponants.Minute}:{dateComponants.Second}";
                    }

					var request = UNNotificationRequest.FromIdentifier(requestId, content, trigger);

					UNUserNotificationCenter.Current.AddNotificationRequest(request, err =>
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
					SoundName = soundEnum == Constants.NotificationSounds.SystemDefault ? UILocalNotification.DefaultSoundName : Constants.NotificaitonSoundsMap[soundEnum],
					TimeZone = NSTimeZone.LocalTimeZone,
					ApplicationIconBadgeNumber = -1,
					Category = Constants.ExerciseNotificationCategoryId,
					UserInfo = notificationDictionary,
					AlertTitle = "Time to Move"
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

