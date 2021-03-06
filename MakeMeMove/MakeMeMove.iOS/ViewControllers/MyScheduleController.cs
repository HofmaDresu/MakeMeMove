using System;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.ViewControllers.Base;
using MakeMeMove.Model;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class MyScheduleController : BaseTabbedViewController
    {
		private ExerciseSchedule _exerciseSchedule;
		private FloatingButton _changeScheduleButton;

        public MyScheduleController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

		    if (Data.IsFirstRun())
		    {
		        ServiceManager.StartNotificationService(false);
                Data.MarkFirstRun();
		    }

            if (this.RevealViewController() == null) return;

            MenuButton.TintColor = UIColor.White;

            AddButtons();
			StatusSwitch.TintColor = Colors.InteractableTextColor;
            StatusSwitch.SelectedSegment = ServiceManager.NotificationServiceIsRunning() ? 1 : 0;
        }

        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            this.RevealViewController().RevealToggleAnimated(true);
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
            MenuButton.Clicked += MenuButton_Clicked;
            _exerciseSchedule = Data.GetExerciseSchedule();
			StartTime.Text = _exerciseSchedule.StartTime.ToLongTimeString();
			EndTime.Text = _exerciseSchedule.EndTime.ToLongTimeString();
			ReminderPeriod.Text = _exerciseSchedule.PeriodDisplayString;

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
            MenuButton.Clicked -= MenuButton_Clicked;
        }
	}
}