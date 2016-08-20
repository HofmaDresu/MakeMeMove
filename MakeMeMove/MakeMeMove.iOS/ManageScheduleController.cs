using Foundation;
using System;
using UIKit;
using MakeMeMove.iOS.Controls;

namespace MakeMeMove.iOS
{
	public partial class ManageScheduleController : BaseViewController
    {
		private FloatingButton _saveButton;
		private FloatingButton _cancelButton;

        public ManageScheduleController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			AddButtons();
		}

		private void AddButtons()
		{
			_saveButton = new FloatingButton("Save");
			_saveButton.TranslatesAutoresizingMaskIntoConstraints = false;
			View.Add(_saveButton);

			_cancelButton = new FloatingButton("Cancel");
			_cancelButton.TranslatesAutoresizingMaskIntoConstraints = false;
			View.Add(_cancelButton);


			_saveButton.TopAnchor.ConstraintEqualTo(EndTime.BottomAnchor, 20).Active = true;
			_saveButton.LeftAnchor.ConstraintEqualTo(EndTime.LeftAnchor).Active = true;
			_saveButton.WidthAnchor.ConstraintEqualTo(_saveButton.Frame.Width).Active = true;

			_cancelButton.TopAnchor.ConstraintEqualTo(EndTime.BottomAnchor, 20).Active = true;
			_cancelButton.LeftAnchor.ConstraintEqualTo(_saveButton.RightAnchor, 20).Active = true;
			_cancelButton.WidthAnchor.ConstraintEqualTo(_cancelButton.Frame.Width).Active = true;
		}

		private void SaveButtonTouchUpInside(object sender, EventArgs e)
		{
			ServiceManager.RestartNotificationServiceIfNeeded();
			NavigationController.PopViewController(true);
		}

		private void CancelButtonTouchUpInside(object sender, EventArgs e)
		{
			NavigationController.PopViewController(true);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			_saveButton.TouchUpInside += SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside += CancelButtonTouchUpInside;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			_saveButton.TouchUpInside -= SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside -= CancelButtonTouchUpInside;
		}

		private void CancelChanges(object sender, EventArgs e)
		{
			DismissViewController(true, null);
		}
    }
}