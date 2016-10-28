using System;
using System.IO;
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


            OpenFudistLabel.Text = "Open Fudist";
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

            OpenFudistView.TouchUpInside += NotImplementedAlert;
            ViewHistoryView.TouchUpInside += NavToExerciseHistory;
            SignInOutView.TouchUpInside += OnSignInOut;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OpenFudistView.TouchUpInside -= NotImplementedAlert;
            ViewHistoryView.TouchUpInside -= NavToExerciseHistory;
            SignInOutView.TouchUpInside -= OnSignInOut;
        }

        private void NavToExerciseHistory(object sender, EventArgs e)
        {

            var regController = AppDelegate.ExerciseHistoryStoryboard.InstantiateInitialViewController();

            this.RevealViewController().RevealToggleAnimated(true);

            PresentViewController(regController, true, () => { });
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
                
            }
        }

        private void NotImplementedAlert(object sender, EventArgs e)
        {
            var alert = new UIAlertView("Not implemented", "This feature is not yet implemented. Please try again in a future version",
                       null, "OK", null);

            alert.Show();
        }
    }
}