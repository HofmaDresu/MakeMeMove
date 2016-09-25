using System;
using MakeMeMove.iOS.Helpers;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers.Base
{
	public abstract class BaseTabbedViewController : BaseViewController
	{
		protected BaseTabbedViewController(IntPtr handle) : base (handle)
        {
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

		    if (NavigationController != null && NavigationController.NavigationBar != null)
            {
                NavigationController.NavigationBar.BarTintColor = FudistColors.PrimaryColor;
                NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes
                {
                    ForegroundColor = UIColor.White
                };
			    NavigationItem.HidesBackButton = true;
            }

		    if (TabBarController != null && TabBarController.TabBar != null)
            {
                TabBarController.TabBar.TintColor = FudistColors.InteractableTextColor;
            }
		}
	}
}


