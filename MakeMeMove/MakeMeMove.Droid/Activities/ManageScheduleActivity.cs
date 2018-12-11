using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Humanizer;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Droid.Fragments;
using MakeMeMove.Model;
using static Android.App.TimePickerDialog;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Manage Schedule", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageScheduleActivity : BaseActivity, IOnTimeSetListener
    {
        private Spinner _scheduleTypeSpinner;
        private Spinner _reminderPeriodSpinner;
        private Button _saveButton;
        private Button _cancelButton;
        private View _startTimeContainer;
        private TextView _startTimeText;
        private View _endTimeContainer;
        private TextView _endTimeText;

        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private readonly UserNotification _userNotification = new UserNotification();
        private ExerciseSchedule _exerciseSchedule;

        private enum TimePickerType
        {
            None, Start, End
        }

        //TODO: Save and restore
        private TimePickerType _selectedPicker = TimePickerType.None;
        private DateTime _selectedStartTime;
        private DateTime _selectedEndTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _exerciseSchedule = Data.GetExerciseSchedule();


            SetContentView(Resource.Layout.ManageSchedule);
            _scheduleTypeSpinner = FindViewById<Spinner>(Resource.Id.ScheduleTypeSpinner);
            _reminderPeriodSpinner = FindViewById<Spinner>(Resource.Id.ReminderSpinner);
            _saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            _cancelButton = FindViewById<Button>(Resource.Id.CancelButton);
            _startTimeContainer = FindViewById(Resource.Id.StartTimeContainer);
            _startTimeText = FindViewById<TextView>(Resource.Id.StartTimeText);
            _endTimeContainer = FindViewById(Resource.Id.EndTimeContainer);
            _endTimeText = FindViewById<TextView>(Resource.Id.EndTimeText);


            InitializePickers();

            SetData();

            _cancelButton.Click += (s, e) => Finish();
            _saveButton.Click += (s, e) => SaveData();
        }

        private void SetData()
        {
            _reminderPeriodSpinner.SetSelection((int) _exerciseSchedule.Period);
            _scheduleTypeSpinner.SetSelection((int)_exerciseSchedule.Type);
            _startTimeText.Text = _exerciseSchedule.StartTime.ToShortTimeString();
            _endTimeText.Text = _exerciseSchedule.EndTime.ToShortTimeString();
            _selectedStartTime = _exerciseSchedule.StartTime;
            _selectedEndTime = _exerciseSchedule.EndTime;
        }

        private void InitializePickers()
        {
			var periodList = PickerListHelper.GetExercisePeriods();
            var scheduleTypeList = PickerListHelper.GetScheduleTypes();

            _reminderPeriodSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, periodList);
            _scheduleTypeSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, scheduleTypeList);

            _startTimeContainer.Click += _startTimeContainer_Click;
            _endTimeContainer.Click += _endTimeContainer_Click;
        }

        private void _startTimeContainer_Click(object sender, EventArgs e)
        {
            _selectedPicker = TimePickerType.Start;
            new TimePickerFragment(_selectedStartTime.Hour, _selectedStartTime.Minute).Show(SupportFragmentManager, "StartTimePicker");
        }

        private void _endTimeContainer_Click(object sender, EventArgs e)
        {
            _selectedPicker = TimePickerType.End;
            new TimePickerFragment(_selectedEndTime.Hour, _selectedEndTime.Minute).Show(SupportFragmentManager, "EndTimePicker");
        }

        private void SaveData()
        {
            _exerciseSchedule.Period = (SchedulePeriod)_reminderPeriodSpinner.SelectedItemPosition;
            _exerciseSchedule.Type = (ScheduleType)_scheduleTypeSpinner.SelectedItemPosition;


            if (_selectedStartTime >= _selectedEndTime)
            {
                _userNotification.ShowValidationErrorPopUp(this, Resource.String.TimeRangeValidation);
                return;
            }

            _exerciseSchedule.StartTime = _selectedStartTime;
            _exerciseSchedule.EndTime = _selectedEndTime;

            Data.SaveExerciseSchedule(_exerciseSchedule);
            _exerciseSchedule = Data.GetExerciseSchedule();
            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);

            Finish();
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            try
            {
                switch (_selectedPicker)
                {
                    case TimePickerType.Start:
                        _selectedStartTime = new DateTime(_selectedStartTime.Year, _selectedStartTime.Month, _selectedStartTime.Day, hourOfDay, minute, 0);
                        _startTimeText.Text = _selectedStartTime.ToShortTimeString();
                        break;
                    case TimePickerType.End:
                        _selectedEndTime = new DateTime(_selectedEndTime.Year, _selectedEndTime.Month, _selectedEndTime.Day, hourOfDay, minute, 0);
                        _endTimeText.Text = _selectedEndTime.ToShortTimeString();
                        break;
                    case TimePickerType.None:
                    default:
                        break;
                }
            }
            finally
            {
                _selectedPicker = TimePickerType.None;
            }
        }
    }
}