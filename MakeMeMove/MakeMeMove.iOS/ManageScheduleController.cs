using Foundation;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
	public partial class ManageScheduleController : BaseViewController
    {
        public ManageScheduleController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
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

		protected override UINavigationBar GetNavBar() => NavBar;

		private void CancelChanges(object sender, EventArgs e)
		{
			DismissViewController(true, null);
		}
    }
}