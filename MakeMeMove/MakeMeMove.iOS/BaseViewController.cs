using System;
using System.IO;
using Foundation;
using MakeMeMove.iOS.Helpers;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS
{
	public abstract class BaseViewController : UIViewController, IUINavigationBarDelegate
	{
		protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));
		protected readonly ExerciseServiceManager ServiceManager;

		protected BaseViewController(IntPtr handle) : base (handle)
        {
			ServiceManager = new ExerciseServiceManager(Data);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.BackgroundColor = FudistColors.MainBackgroundColor;
			NavigationController.NavigationBar.BarTintColor = FudistColors.PrimaryColor;
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
			{
				ForegroundColor = UIColor.White
			};

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			ServiceManager.RestartNotificationServiceIfNeeded();
		}
	}
}


