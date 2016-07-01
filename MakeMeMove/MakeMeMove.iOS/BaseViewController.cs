using System;
using Foundation;
using UIKit;

namespace MakeMeMove.iOS
{
	public abstract class BaseViewController : UIViewController, IUINavigationBarDelegate
	{
		protected BaseViewController(IntPtr handle) : base (handle)
        {
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			GetNavBar().Delegate = this;
		}

		[Export("positionForBar:")]
		public UIBarPosition PositionForBar(UIBarPositioning id)
		{
			return UIBarPosition.TopAttached;
		}

		protected abstract UINavigationBar GetNavBar();
	}
}


