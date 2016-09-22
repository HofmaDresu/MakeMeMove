using Foundation;
using System;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class MenuViewController : UIViewController
    {
        public MenuViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = FudistColors.PrimaryColor;
            UserNameView.BackgroundColor = FudistColors.TertiaryColor;


            MenuBackgroundView.BackgroundColor = FudistColors.MainBackgroundColor;


            //TODO: Use real value
            UserNameLabel.Text = "Fake UserName";
            SignInOutLabel.Text = "Sign Out";
            OpenFudistLabel.Text = "Open Fudist";
        }
    }
}