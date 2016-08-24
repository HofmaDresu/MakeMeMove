using Foundation;
using System;
using UIKit;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Models;
using System.Collections.Generic;
using System.Linq;
using MakeMeMove.Model;
using MakeMeMove.iOS.Helpers;

namespace MakeMeMove.iOS
{
	public partial class ManageScheduleController : BaseViewController
    {
		private FloatingButton _saveButton;
		private FloatingButton _cancelButton;
		private List<List<string>> _availableTimes = new List<List<string>>();
		private ExerciseSchedule _schedule;
		private UIPickerView _schedulePicker;
		private UIPickerView _startTimePicker;
		private UIPickerView _endTimePicker;

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

			_schedule = Data.GetExerciseSchedule();

			_schedulePicker = MirroredPicker.Create(new PickerModel(PickerListHelper.GetExercisePeriods()), 
			                      ReminderPeriod, doneAction: null);

			_startTimePicker = MirroredPicker.Create(new PickerModel(_availableTimes), StartTime, HandleTimeSet, doneAction: null);

			_endTimePicker = MirroredPicker.Create(new PickerModel(_availableTimes), EndTime, HandleTimeSet, doneAction: null);

			AddButtons();
			PopulateData();

			var pickerTextFields = View.Subviews.OfType<PickerUITextField>().ToArray();
			FudistColors.SetTextPrimaryColor(pickerTextFields);
		}

		private void PopulateData()
		{
			ReminderPeriod.Text = _schedule.PeriodDisplayString;
			_schedulePicker.Select((int)_schedule.Period, 0, false);

			var startHour = GetCivilianHour(_schedule.StartTime.Hour);
			var startMinute = _schedule.StartTime.Minute.ToString("D2");
			var startMeridian = GetMeridian(_schedule.StartTime.Hour);

			StartTime.Text = $"{startHour}:{startMinute} {startMeridian}";
			_startTimePicker.Select(startHour - 1, 0, false);
			_startTimePicker.Select(startMinute == "00" ? 0 : 1, 1, false);
			_startTimePicker.Select(_schedule.StartTime.Hour < 12 ? 0 : 1, 2, false);

			var endHour = GetCivilianHour(_schedule.EndTime.Hour);
			var endMinute = _schedule.EndTime.Minute.ToString("D2");
			var endMeridian = GetMeridian(_schedule.EndTime.Hour);

			EndTime.Text = $"{endHour}:{endMinute} {endMeridian}";
			_endTimePicker.Select(endHour - 1, 0, false);
			_endTimePicker.Select(endMinute == "00" ? 0 : 1, 1, false);
			_endTimePicker.Select(_schedule.EndTime.Hour < 12 ? 0 : 1, 2, false);
		}

		private int GetCivilianHour(int hour)
		{
			return hour > 12 ? hour - 12 : hour;
		}

		private string GetMeridian(int hour)
		{
			return hour >= 12 ? "PM" : "AM";
		}

		private string HandleTimeSet(IList<string> arg)
		{
			return $"{arg[0]}:{arg[1]} {arg[2]}";
		}

		private void AddButtons()
		{
			_saveButton = new FloatingButton("Save");
			View.Add(_saveButton);

			_cancelButton = new FloatingButton("Cancel");
			View.Add(_cancelButton);


			_saveButton.TopAnchor.ConstraintEqualTo(EndTime.BottomAnchor, 20).Active = true;
			_saveButton.LeftAnchor.ConstraintEqualTo(EndTime.LeftAnchor).Active = true;

			_cancelButton.TopAnchor.ConstraintEqualTo(EndTime.BottomAnchor, 20).Active = true;
			_cancelButton.LeftAnchor.ConstraintEqualTo(_saveButton.RightAnchor, 20).Active = true;
		}

		private void SaveButtonTouchUpInside(object sender, EventArgs e)
		{
			_schedule.Period = (SchedulePeriod)(int)_schedulePicker.SelectedRowInComponent(0);

			var startHour = 1 +(int)(_startTimePicker.SelectedRowInComponent(0) + (12 * _startTimePicker.SelectedRowInComponent(2)));
			var startMinute = (int)_startTimePicker.SelectedRowInComponent(1) * 30;
			var startTime = new DateTime(1, 1, 1, startHour, startMinute, 0);

			var endHour = 1 + (int)(_endTimePicker.SelectedRowInComponent(0) + (12 * _endTimePicker.SelectedRowInComponent(2)));
			var endMinute = (int)_endTimePicker.SelectedRowInComponent(1) * 30;
			var endTime = new DateTime(1, 1, 1, endHour, endMinute, 0);

			if (startTime >= endTime)
			{
				GeneralAlertDialogs.ShowValidationErrorPopUp(this, "Please make sure your start time is before your end time.");
				return;
			}

			_schedule.StartTime = startTime;
			_schedule.EndTime = endTime;

			Data.SaveExerciseSchedule(_schedule);
			
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