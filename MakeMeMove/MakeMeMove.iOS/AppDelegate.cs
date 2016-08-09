using System;
using System.IO;
using Foundation;
using MakeMeMove.Model;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			var nextAction = new UIMutableUserNotificationAction
			{
				Identifier = Constants.NextId,
				Title = "Change",
				ActivationMode = UIUserNotificationActivationMode.Background,
				Destructive = false,
				AuthenticationRequired = false
			};

			var unregisteredExerciseCategory = new UIMutableUserNotificationCategory
			{
				Identifier = Constants.UnregisteredExerciseCategoryId
			};

			var nextOnlyActionArray = new UIUserNotificationAction[] { nextAction };
			unregisteredExerciseCategory.SetActions(nextOnlyActionArray, UIUserNotificationActionContext.Default);
			unregisteredExerciseCategory.SetActions(nextOnlyActionArray, UIUserNotificationActionContext.Minimal);

			var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
				UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, 
				new NSSet(unregisteredExerciseCategory)
			);

			application.RegisterUserNotificationSettings(notificationSettings);

			return true;
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("CHANGE", UIAlertActionStyle.Default, null));

			if (notification.Category == Constants.UnregisteredExerciseCategoryId)
			{
				okayAlertController.AddAction(UIAlertAction.Create("DISMISS", UIAlertActionStyle.Cancel, _ => ChangeExercise(notification)));
			}

			Window.RootViewController.PresentViewController(okayAlertController, true, null);
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, UILocalNotification localNotification, NSDictionary responseInfo, System.Action completionHandler)
		{
			if (actionIdentifier == Constants.NextId)
			{
				ChangeExercise(localNotification);
			}

			completionHandler?.Invoke();
		}

		void ChangeExercise(UILocalNotification localNotification)
		{
			var exerciseName = localNotification.UserInfo[Constants.ExerciseName].ToString();
			var exerciseQuantity = int.Parse(localNotification.UserInfo[Constants.ExerciseQuantity].ToString());

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

			UserNotification.CreateNotification(DateTime.Now.AddSeconds(5), nextExercise);
		}
	}
}


