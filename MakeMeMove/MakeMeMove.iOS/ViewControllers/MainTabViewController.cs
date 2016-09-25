using System;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class MainTabViewController : UITabBarController
    {
        public MainTabViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			if (this.RevealViewController() == null) return;
            
			View.AddGestureRecognizer(this.RevealViewController().PanGestureRecognizer);
		}
    }
}