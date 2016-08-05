using Foundation;
using System;
using UIKit;
using MakeMeMove.Model;

namespace MakeMeMove.iOS
{
    public partial class MyScheduleController : BaseViewController
    {
		private ExerciseSchedule _exerciseSchedule;
        public MyScheduleController (IntPtr handle) : base (handle)
        {
			_exerciseSchedule = Data.GetExerciseSchedule();
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			StatusSwitch.TouchUpInside += StatusSwitch_TouchUpInside;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			StartTime.Text = _exerciseSchedule.StartTime.ToLongTimeString();
			EndTime.Text = _exerciseSchedule.EndTime.ToLongTimeString();
			ReminderPeriod.Text = _exerciseSchedule.PeriodDisplayString;
			StatusSwitch.On = ServiceManager.NotificationServiceIsRunning();
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

		public override void ViewWillUnload()
		{
			base.ViewWillUnload();
			StatusSwitch.TouchUpInside -= StatusSwitch_TouchUpInside;
		}
	}
}