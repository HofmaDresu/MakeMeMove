using System;
using System.IO;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers.Base
{
	public abstract class BaseViewController : UIViewController
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


			var labels = View.Subviews.OfType<UILabel>().ToArray();
			FudistColors.SetTextPrimaryColor(labels);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			ServiceManager.RestartNotificationServiceIfNeeded();
		}
	}
}


