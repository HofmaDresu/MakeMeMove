using System;
using System.IO;
using Foundation;
using MakeMeMove.iOS.Helpers;
using SQLite;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class MenuViewController : UIViewController
    {

        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));

        public MenuViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = FudistColors.PrimaryColor;
            UserNameView.BackgroundColor = FudistColors.TertiaryColor;


            MenuBackgroundView.BackgroundColor = FudistColors.MainBackgroundColor;


            OpenFudistLabel.Text = "See Fudist in the App Store";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (_data.UserIsSignedIn())
            {
                UserNameLabel.Text = _data.GetUserName();
                SignInOutLabel.Text = "Sign Out";
                UserNameViewHeightConstraint.Constant = 91;
            }
            else
            {
                UserNameLabel.Text = string.Empty;
                SignInOutLabel.Text = "Sign In";
                UserNameViewHeightConstraint.Constant = 0;
            }

            OpenFudistView.TouchUpInside += OpenFudistClicked;
            ViewHistoryView.TouchUpInside += NavToExerciseHistory;
            SignInOutView.TouchUpInside += NotImplementedAlert;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OpenFudistView.TouchUpInside -= OpenFudistClicked;
            ViewHistoryView.TouchUpInside -= NavToExerciseHistory;
            SignInOutView.TouchUpInside -= NotImplementedAlert;
        }

        private void NavToExerciseHistory(object sender, EventArgs e)
        {

            var regController = AppDelegate.ExerciseHistoryStoryboard.InstantiateInitialViewController();

            this.RevealViewController().RevealToggleAnimated(true);

            PresentViewController(regController, true, () => { });
        }

        private void OpenFudistClicked(object sender, EventArgs e)
        {
            var iTunesLink = "https://itunes.apple.com/us/app/fudist/id885638462?mt8";
            UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(iTunesLink));
        }

        private void NotImplementedAlert(object sender, EventArgs e)
        {
            var alert = new UIAlertView("Not implemented", "This feature is not yet implemented. Please try again in a future version",
                       null, "OK", null);

            alert.Show();
        }
    }
}