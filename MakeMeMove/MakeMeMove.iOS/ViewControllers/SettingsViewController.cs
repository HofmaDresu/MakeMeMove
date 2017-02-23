using CoreGraphics;
using Foundation;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.ViewControllers.Base;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class SettingsViewController : BaseViewController
    {
        public SettingsViewController (IntPtr handle) : base (handle)
        {
            ScreenName = "Settings";
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavBar.Translucent = false;
            NavBar.BarTintColor = FudistColors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            var statusBarColor = new UIView(new CGRect(0, 0, View.Frame.Width, 20))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            View.Add(statusBarColor);

            BackButton.Clicked += BackButton_Clicked;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BackButton.TintColor = UIColor.White;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            BackButton.Clicked -= BackButton_Clicked;
        }

        private void BackButton_Clicked(object sender, EventArgs e)
        {
            DismissViewController(true, () => { });
        }
    }
}