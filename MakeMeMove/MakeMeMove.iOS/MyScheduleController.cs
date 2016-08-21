using Foundation;
using System;
using System.Linq;
using UIKit;
using MakeMeMove.Model;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;

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

			var labels = View.Subviews.OfType<UILabel>();
			FudistColors.SetTextPrimaryColor(labels.ToArray());

			AddButtons();
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
			StatusSwitch.On = ServiceManager.NotificationServiceIsRunning();

			StatusSwitch.TouchUpInside += StatusSwitch_TouchUpInside;
			_changeScheduleButton.TouchUpInside += ChangeScheduleButton_TouchUpInside;
		}

		void ChangeScheduleButton_TouchUpInside(object sender, EventArgs e)
		{
			PerformSegue("ManageScheduleSegue", this);
		}

		void StatusSwitch_TouchUpInside(object sender, EventArgs e)
		{
			if (StatusSwitch.On)
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
			StatusSwitch.TouchUpInside -= StatusSwitch_TouchUpInside;
			_changeScheduleButton.TouchUpInside -= ChangeScheduleButton_TouchUpInside;
		}
	}
}