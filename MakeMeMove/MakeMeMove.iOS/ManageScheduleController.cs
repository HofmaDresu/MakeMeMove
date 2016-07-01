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

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			CancelButton.TouchUpInside += CancelChanges;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			CancelButton.TouchUpInside -= CancelChanges;
		}

		[Export("positionForBar:")]
		public UIBarPosition PositionForBar(UIBarPositioning id)
		{
			return UIBarPosition.TopAttached;
		}

		private void CancelChanges(object sender, EventArgs e)
		{
			DismissViewController(true, null);
		}
    }
}