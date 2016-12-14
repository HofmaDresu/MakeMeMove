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
        private const int UserNameViewFullHeight = 91;
        private const string SignIn = "Sign In";
        private const string SignOut = "Sign Out";

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
                SignInOutLabel.Text = SignOut;
                UserNameViewHeightConstraint.Constant = UserNameViewFullHeight;
            }
            else
            {
                UserNameLabel.Text = string.Empty;
                SignInOutLabel.Text = SignIn;
                UserNameViewHeightConstraint.Constant = 0;
            }

            OpenFudistView.TouchUpInside += OpenFudistClicked;
            ViewHistoryView.TouchUpInside += NavToExerciseHistory;
            SignInOutView.TouchUpInside += OnSignInOut;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OpenFudistView.TouchUpInside -= OpenFudistClicked;
            ViewHistoryView.TouchUpInside -= NavToExerciseHistory;
            SignInOutView.TouchUpInside -= OnSignInOut;
        }

        private void NavToExerciseHistory(object sender, EventArgs e)
        {

            if (_data.UserIsPremium())
            {
                var regController = AppDelegate.ExerciseHistoryStoryboard.InstantiateInitialViewController();

                this.RevealViewController().RevealToggleAnimated(true);

                PresentViewController(regController, true, () => { });
            }
            else if (_data.UserIsSignedIn())
            {
                var alert = new UIAlertView("Premium Account Needed", "Your current account is not subscribed to Fudist Premium. Please double check your subscription status and try again.",
                           null, "OK", null);

                alert.Show();
            }
            else
            {
                var alert = new UIAlertView("Account Needed", "You must sign in as a Fudist Premium user to access your exercise history. Would you like to sign in?",
                           null, "No", "Yes");
                alert.Clicked += (o, args) =>
                {
                    if (args.ButtonIndex == 1)
                    {
                        OnSignInOut(o, args);
                    }
                };

                alert.Show();
            }
        }

        private void OnSignInOut(object sender, EventArgs e)
        {
            if (_data.UserIsSignedIn())
            {
                _data.SignUserOut();
                UserNameLabel.Text = string.Empty;
                SignInOutLabel.Text = SignIn;
                UserNameViewHeightConstraint.Constant = 0;
            }
            else
            {
                var regController = AppDelegate.LoginStoryboard.InstantiateInitialViewController();

                this.RevealViewController().RevealToggleAnimated(true);

                PresentViewController(regController, true, () => { });
            }
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