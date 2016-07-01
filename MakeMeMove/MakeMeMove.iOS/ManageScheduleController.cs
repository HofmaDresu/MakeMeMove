using Foundation;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
	public partial class ManageScheduleController : UIViewController, IUINavigationBarDelegate
    {
        public ManageScheduleController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			NavBar.Delegate = this;
		}

		[Export("positionForBar:")]
		public UIBarPosition PositionForBar(UIBarPositioning id)
		{
			return UIBarPosition.TopAttached;
		}
    }
}