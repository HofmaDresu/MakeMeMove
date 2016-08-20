using Foundation;
using System;
using UIKit;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Models;
using System.Collections.Generic;
using System.Linq;
using MakeMeMove.Model;

namespace MakeMeMove.iOS
{
	public partial class ManageScheduleController : BaseViewController
    {
		private FloatingButton _saveButton;
		private FloatingButton _cancelButton;
		private List<List<string>> _availableTimes = new List<List<string>>();

        public ManageScheduleController (IntPtr handle) : base (handle)
        {
			var hours = Enumerable.Range(1, 12).Select(h => h.ToString()).ToList();
			var minutes = new List<string> { "00", "30"};
			var meridian = new List<string> { "AM", "PM" };
			_availableTimes.Add(hours);
			_availableTimes.Add(minutes);
			_availableTimes.Add(meridian);
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			MirroredPicker.Create(new PickerModel(PickerListHelper.GetExercisePeriods()), 
			                      ReminderPeriod,  doneAction: null);

			MirroredPicker.Create(new PickerModel(_availableTimes), StartTime, HandleTimeSet, doneAction: null);

			MirroredPicker.Create(new PickerModel(_availableTimes), EndTime, HandleTimeSet, doneAction: null);

			AddButtons();
		}

		private string HandleTimeSet(IList<string> arg)
		{
			return $"{arg[0]}:{arg[1]} {arg[2]}";
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