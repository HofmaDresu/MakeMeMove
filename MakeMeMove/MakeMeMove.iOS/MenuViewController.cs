using Foundation;
using System;
using System.IO;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS
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


            OpenFudistLabel.Text = "Open Fudist";
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

            OpenFudistView.TouchUpInside += NotImplementedAlert;
            ViewHistoryView.TouchUpInside += NotImplementedAlert;
            SignInOutView.TouchUpInside += NotImplementedAlert;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            OpenFudistView.TouchUpInside -= NotImplementedAlert;
            ViewHistoryView.TouchUpInside -= NotImplementedAlert;
            SignInOutView.TouchUpInside -= NotImplementedAlert;
        }

        private void NotImplementedAlert(object sender, EventArgs e)
        {
            var alert = new UIAlertView("Not implemented", "This feature is not yet implemented. Please try again in a future version",
                       null, "OK", null);

            alert.Show();
        }
    }
}