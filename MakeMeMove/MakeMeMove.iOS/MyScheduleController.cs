using Foundation;
using System;
using System.Linq;
using UIKit;
using MakeMeMove.Model;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using SWRevealViewControllerBinding;

namespace MakeMeMove.iOS
{
    public partial class MyScheduleController : BaseViewController
    {
		private ExerciseSchedule _exerciseSchedule;
		private FloatingButton _changeScheduleButton;

        public MyScheduleController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            if (this.RevealViewController() == null) return;

            MenuButton.Clicked += (sender, e) => this.RevealViewController().RevealToggleAnimated(true);
		    MenuButton.TintColor = UIColor.White;

            AddButtons();
			StatusSwitch.TintColor = FudistColors.InteractableTextColor;
		}

		private void AddButtons()
		{
			_changeScheduleButton = new FloatingButton("Change Schedule");
			View.Add(_changeScheduleButton);

			_changeScheduleButton.TopAnchor.ConstraintEqualTo(ReminderPeriod.BottomAnchor, 45).Active = true;
			_changeScheduleButton.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;

		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			_exerciseSchedule = Data.GetExerciseSchedule();
			StartTime.Text = _exerciseSchedule.StartTime.ToLongTimeString();
			EndTime.Text = _exerciseSchedule.EndTime.ToLongTimeString();
			ReminderPeriod.Text = _exerciseSchedule.PeriodDisplayString;
			StatusSwitch.SelectedSegment = ServiceManager.NotificationServiceIsRunning() ? 1 : 0;

			StatusSwitch.ValueChanged += StatusSwitch_ValueChanged;
			_changeScheduleButton.TouchUpInside += ChangeScheduleButton_TouchUpInside;
		}

		void ChangeScheduleButton_TouchUpInside(object sender, EventArgs e)
		{
			PerformSegue("ManageScheduleSegue", this);
		}

		void StatusSwitch_ValueChanged(object sender, EventArgs e)
		{
			if (StatusSwitch.SelectedSegment == 1)
			{
				ServiceManager.StartNotificationService();
			}
			else
			{
				ServiceManager.StopNotificationService();
			}

		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StatusSwitch.ValueChanged -= StatusSwitch_ValueChanged;
			_changeScheduleButton.TouchUpInside -= ChangeScheduleButton_TouchUpInside;
		}
	}
}