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
            View.BackgroundColor = FudistColors.MainBackgroundColor;
            var labels = View.Subviews.OfType<UILabel>().ToArray();
            FudistColors.SetTextPrimaryColor(labels);
        }
    }
}