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
			StartTime.Text = _exerciseSchedule.StartTime.ToLongTimeString();
			EndTime.Text = _exerciseSchedule.EndTime.ToLongTimeString();
			ReminderPeriod.Text = _exerciseSchedule.PeriodDisplayString;
		}

		protected override UINavigationBar GetNavBar() => NavBar;
    }
}