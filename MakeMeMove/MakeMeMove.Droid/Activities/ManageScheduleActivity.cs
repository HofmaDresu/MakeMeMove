using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using Humanizer;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Manage Schedule", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageScheduleActivity : BaseActivity
    {
        private Spinner _scheduleTypeSpinner;
        private Spinner _reminderPeriodSpinner;
        private Spinner _startHourSpinner;
        private Spinner _startMinuteSpinner;
        private Spinner _startMeridianSpinner;
        private Spinner _endHourSpinner;
        private Spinner _endMinuteSpinner;
        private Spinner _endMeridianSpinner;
        private Button _saveButton;
        private Button _cancelButton;

        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private readonly UserNotification _userNotification = new UserNotification();
        private ExerciseSchedule _exerciseSchedule;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _exerciseSchedule = Data.GetExerciseSchedule();


            SetContentView(Resource.Layout.ManageSchedule);
            _scheduleTypeSpinner = FindViewById<Spinner>(Resource.Id.ScheduleTypeSpinner);
            _reminderPeriodSpinner = FindViewById<Spinner>(Resource.Id.ReminderSpinner);
            _startHourSpinner = FindViewById<Spinner>(Resource.Id.StartHourSpinner);
            _startMinuteSpinner = FindViewById<Spinner>(Resource.Id.StartMinuteSpinner);
            _startMeridianSpinner = FindViewById<Spinner>(Resource.Id.StartMeridianSpinner);
            _endHourSpinner = FindViewById<Spinner>(Resource.Id.EndHourSpinner);
            _endMinuteSpinner = FindViewById<Spinner>(Resource.Id.EndMinuteSpinner);
            _endMeridianSpinner = FindViewById<Spinner>(Resource.Id.EndMeridianSpinner);
            _saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            _cancelButton = FindViewById<Button>(Resource.Id.CancelButton);


            InitializePickers();

            SetPickerData();

            _cancelButton.Click += (s, e) => Finish();
            _saveButton.Click += (s, e) => SaveData();
        }

        private void SetPickerData()
        {
            _reminderPeriodSpinner.SetSelection((int) _exerciseSchedule.Period);
            _scheduleTypeSpinner.SetSelection((int)_exerciseSchedule.Type);

            var civilianModifiedStartHour = TimeUtility.GetCivilianModifiedStartHour(_exerciseSchedule.StartTime);

            _startHourSpinner.SetSelection(civilianModifiedStartHour - 1);
            _startMinuteSpinner.SetSelection(_exerciseSchedule.StartTime.Minute / 15);
            _startMeridianSpinner.SetSelection(_exerciseSchedule.StartTime.Hour < 12 ? 0 : 1);


            var civilianModifiedEndHour = TimeUtility.GetCivilianModifiedStartHour(_exerciseSchedule.EndTime);

            _endHourSpinner.SetSelection(civilianModifiedEndHour -1);
            _endMinuteSpinner.SetSelection(_exerciseSchedule.EndTime.Minute / 15);
            _endMeridianSpinner.SetSelection(_exerciseSchedule.EndTime.Hour < 12 ? 0 : 1);
        }

        private void InitializePickers()
        {
			var periodList = PickerListHelper.GetExercisePeriods();
            var scheduleTypeList = PickerListHelper.GetScheduleTypes();

            _reminderPeriodSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, periodList);
            _scheduleTypeSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, scheduleTypeList);

            var hourChoices = Enumerable.Range(1, 12).ToList();
            var minuteChoices = new List<string> {"00", "15", "30", "45"};
            var meridianChoices = new List<string> {Resources.GetString(Resource.String.AM), Resources.GetString(Resource.String.PM) };


            _startHourSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, hourChoices);
            _startMinuteSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, minuteChoices);
            _startMeridianSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, meridianChoices);
            _endHourSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, hourChoices);
            _endMinuteSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, minuteChoices);
            _endMeridianSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, meridianChoices);
        }

        private void SaveData()
        {
            _exerciseSchedule.Period = (SchedulePeriod)_reminderPeriodSpinner.SelectedItemPosition;
            _exerciseSchedule.Type = (ScheduleType)_scheduleTypeSpinner.SelectedItemPosition;

            var startHour = _startHourSpinner.SelectedItemPosition == 11 ? 0 : _startHourSpinner.SelectedItemPosition + 1;
            var startTime = new DateTime(1, 1, 1, startHour + (12 * _startMeridianSpinner.SelectedItemPosition), _startMinuteSpinner.SelectedItemPosition * 15, 0);

            var endHour = _endHourSpinner.SelectedItemPosition == 11 ? 0 : _endHourSpinner.SelectedItemPosition + 1;
            var endTime = new DateTime(1, 1, 1, endHour + (12 * _endMeridianSpinner.SelectedItemPosition), _endMinuteSpinner.SelectedItemPosition * 15, 0);

            if (startTime >= endTime)
            {
                _userNotification.ShowValidationErrorPopUp(this, Resource.String.TimeRangeValidation);
                return;
            }

            _exerciseSchedule.StartTime = startTime;
            _exerciseSchedule.EndTime = endTime;

            Data.SaveExerciseSchedule(_exerciseSchedule);
            _exerciseSchedule = Data.GetExerciseSchedule();
            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);

            Finish();
        }
    }
}