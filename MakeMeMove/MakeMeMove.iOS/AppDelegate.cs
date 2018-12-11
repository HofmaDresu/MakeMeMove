using System;
using System.IO;
using Foundation;
using SQLite;
using SWRevealViewControllerBinding;
using UIKit;
using UserNotifications;
using Humanizer;
using MakeMeMove.iOS.Helpers;

namespace MakeMeMove.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));
		private readonly ExerciseServiceManager _serviceManager;
        public static UIStoryboard Storyboard;
        public static UIStoryboard ExerciseHistoryStoryboard;
        public static UIStoryboard SettingsStoryboard;
        public static UIViewController InitialViewController;

        public AppDelegate()
		{
			_serviceManager = new ExerciseServiceManager(_data);
		}

		public override UIWindow Window
		{
			get;
			set;
		}

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
		    InstantiateStoryboards();
            InstantiateUserDefaults();
            _data.IncrementRatingCycle();

			UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound, (approved, err) =>
			{
				UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
			});

			var nextAction = UNNotificationAction.FromIdentifier(Constants.NextId, "Change", UNNotificationActionOptions.None);
			var completeAction = UNNotificationAction.FromIdentifier(Constants.CompleteId, "Complete", UNNotificationActionOptions.None);
                
			var actions = new[] { nextAction, completeAction };
			var intentIDs = new string[] { };

			var category = UNNotificationCategory.FromIdentifier(Constants.ExerciseNotificationCategoryId, actions, intentIDs, UNNotificationCategoryOptions.CustomDismissAction);

			var categories = new[] { category };
			UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(categories));

			UINavigationBar.Appearance.BarTintColor = Colors.PrimaryColor;
			UINavigationBar.Appearance.Translucent = false;
			UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            return true;
		}

        private void InstantiateUserDefaults()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            var sound = plist.StringForKey(Constants.UserDefaultsNotificationSoundsKey);
            if (string.IsNullOrWhiteSpace(sound))
            {
                plist.SetString(Constants.NotificationSounds.SystemDefault.Humanize(LetterCasing.Title), Constants.UserDefaultsNotificationSoundsKey);
                plist.Synchronize();
            }
        }

	    private void InstantiateStoryboards()
        {
            Storyboard = UIStoryboard.FromName("Main", null);
            InitialViewController = Storyboard.InstantiateInitialViewController() as SWRevealViewController;
            ExerciseHistoryStoryboard = UIStoryboard.FromName("ExerciseHistory", null);
            SettingsStoryboard = UIStoryboard.FromName("Settings", null);
	    }

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			application.ApplicationIconBadgeNumber = 1;
			application.ApplicationIconBadgeNumber = 0;

			var okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("COMPLETE", UIAlertActionStyle.Default, _ => CompleteExercise(notification)));
			okayAlertController.AddAction(UIAlertAction.Create("CHANGE", UIAlertActionStyle.Default, _ => ChangeExercise(notification)));

			okayAlertController.AddAction(UIAlertAction.Create("DISMISS", UIAlertActionStyle.Cancel, null));

			Window.RootViewController.PresentViewController(okayAlertController, true, null);
		}

		public override void HandleAction(UIApplication application, string actionIdentifier, UILocalNotification localNotification, NSDictionary responseInfo, Action completionHandler)
		{
			application.ApplicationIconBadgeNumber = 1;
			application.ApplicationIconBadgeNumber = 0;

			if (actionIdentifier == Constants.NextId)
			{
				ChangeExercise(localNotification);
			}
			if (actionIdentifier == Constants.CompleteId)
			{
				CompleteExercise(localNotification);
			}

			completionHandler?.Invoke();
		}

		void CompleteExercise(UILocalNotification localNotification)
        {
            var exerciseName = localNotification.UserInfo[Constants.ExerciseName].ToString();
			var exerciseQuantity = int.Parse(localNotification.UserInfo[Constants.ExerciseQuantity].ToString());

            _data.MarkExerciseNotified(exerciseName, exerciseQuantity);
            _data.MarkExerciseCompleted(exerciseName, exerciseQuantity);
            _serviceManager.RestartNotificationService();
		}

		void ChangeExercise(UILocalNotification localNotification)
		{
			var exerciseName = localNotification.UserInfo[Constants.ExerciseName].ToString();
			var exerciseQuantity = int.Parse(localNotification.UserInfo[Constants.ExerciseQuantity].ToString());
			_serviceManager.AddInstantExerciseNotificationAndRestartService(exerciseName, exerciseQuantity);
        }
	}
}


