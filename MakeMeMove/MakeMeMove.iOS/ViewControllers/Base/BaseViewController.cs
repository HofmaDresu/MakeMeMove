using System;
using System.IO;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.Utilities;
using SQLite;
using UIKit;
using Foundation;

namespace MakeMeMove.iOS.ViewControllers.Base
{
	public abstract class BaseViewController : UIViewController
	{
		protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));
		protected readonly ExerciseServiceManager ServiceManager;
        protected string ScreenName { get; set; }

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

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (!string.IsNullOrWhiteSpace(ScreenName))
            {
                UnifiedAnalytics.GetInstance().SendScreenHitOnDefaultTracker(ScreenName);
            }



            if (Data.ShouldAskForRating())
            {
                var okayAlertController = UIAlertController.Create("Enjoying Make Me Move?", "Would you like to let us know what you think by rating us in the App Store?", UIAlertControllerStyle.Alert);
                okayAlertController.AddAction(UIAlertAction.Create("Sure", UIAlertActionStyle.Default, _ => RateApp()));
                okayAlertController.AddAction(UIAlertAction.Create("Not Now", UIAlertActionStyle.Default, _ => RateAppLater()));

                okayAlertController.AddAction(UIAlertAction.Create("Never", UIAlertActionStyle.Cancel, _ => NeverRateApp()));

                PresentViewController(okayAlertController, true, null);
            }
        }

        private void RateApp()
        {
            try
            {
                var iTunesLink = "https://itunes.apple.com/app/viewContentsUserReviews?id=1148192869";
                UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(iTunesLink));
            }
            catch (Exception)
            {
                Data.ResetRatingCycle();
                return;
            }

            Data.PreventRatingCheck();
        }

        private void RateAppLater()
        {
            Data.ResetRatingCycle();
        }

        private void NeverRateApp()
        {
            Data.PreventRatingCheck();
        }
    }
}


