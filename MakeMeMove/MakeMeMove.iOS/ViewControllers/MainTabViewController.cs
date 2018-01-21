using System;
using System.IO;
using SQLite;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class MainTabViewController : UITabBarController
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));
        private readonly ExerciseServiceManager _serviceManager;
        public MainTabViewController (IntPtr handle) : base (handle)
        {
            _serviceManager = new ExerciseServiceManager(_data);
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (this.RevealViewController() == null) return;
            
			View.AddGestureRecognizer(this.RevealViewController().PanGestureRecognizer);
            _serviceManager.RestartNotificationServiceIfNeeded();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
    }
}