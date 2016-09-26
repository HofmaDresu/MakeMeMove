using System;
using CoreGraphics;
using Foundation;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.ViewControllers.Base;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class ExerciseHistoryViewController : UIViewController
    {
        public ExerciseHistoryViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = FudistColors.MainBackgroundColor;
            NavBar.Translucent = false;
            NavBar.BarTintColor = FudistColors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            DateDisplayView.BackgroundColor = FudistColors.MainBackgroundColor;

            SelectedDateLabel.Text = DateTime.Now.Date.ToShortDateString();
            SelectedDateLabel.TextColor = FudistColors.PrimaryColor;

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var statusBarColor = new UIView(new CGRect(0, 0, this.View.Frame.Width, 20))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            View.Add(statusBarColor);

            var dateViewBottomBorder = new UIView(new CGRect(0, DateDisplayView.Frame.Height -1, View.Frame.Width, 1))
            {
                BackgroundColor = FudistColors.PrimaryColor
            };
            DateDisplayView.Add(dateViewBottomBorder);
        }
    }
}