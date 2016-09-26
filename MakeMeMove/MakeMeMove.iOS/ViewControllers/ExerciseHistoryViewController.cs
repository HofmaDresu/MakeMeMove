using System;
using CoreGraphics;
using Foundation;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.ViewControllers.Base;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class ExerciseHistoryViewController : BaseViewController
    {
        public ExerciseHistoryViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavBar.BarTintColor = FudistColors.PrimaryColor;
            NavBar.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            DateDisplayView.BackgroundColor = FudistColors.MainBackgroundColor;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            var topBorder = new UIView(new CGRect(0, 0, this.View.Frame.Width, 20))
            {
                TintColor = FudistColors.PrimaryColor
            };
            View.Add(topBorder);
        }
    }
}