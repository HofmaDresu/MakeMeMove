using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Humanizer;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using SQLite;
using Environment = System.Environment;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Manage Schedule", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageScheduleActivity : Activity
    {
        private Spinner _reminderPeriodSpinner;
        private Spinner _startHourSpinner;
        private Spinner _startMinuteSpinner;
        private Spinner _startMeridianSpinner;
        private Spinner _endHourSpinner;
        private Spinner _endMinuteSpinner;
        private Spinner _endMeridianSpinner;
        private Button _saveButton;
        private Button _cancelButton;

        private readonly Data _data = new Data(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private readonly UserNotification _userNotification = new UserNotification();
        private ExerciseSchedule _exerciseSchedule;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _exerciseSchedule = _data.GetExerciseSchedule();


            SetContentView(Resource.Layout.ManageSchedule);
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

            var civilianModifiedStartHour = (_exerciseSchedule.StartTime.Hour > 11
                ? _exerciseSchedule.StartTime.Hour - 12
                : _exerciseSchedule.StartTime.Hour);

            _startHourSpinner.SetSelection(civilianModifiedStartHour == 0 ? 12 : civilianModifiedStartHour - 1);
            _startMinuteSpinner.SetSelection(_exerciseSchedule.StartTime.Minute == 0 ? 0 : 1);
            _startMeridianSpinner.SetSelection(_exerciseSchedule.StartTime.Hour < 12 ? 0 : 1);


            var civilianModifiedEndHour = (_exerciseSchedule.EndTime.Hour > 11
                ? _exerciseSchedule.EndTime.Hour - 12
                : _exerciseSchedule.EndTime.Hour);

            _endHourSpinner.SetSelection(civilianModifiedEndHour == 0 ? 12 : civilianModifiedEndHour - 1);
            _endMinuteSpinner.SetSelection(_exerciseSchedule.EndTime.Minute == 0 ? 0 : 1);
            _endMeridianSpinner.SetSelection(_exerciseSchedule.EndTime.Hour < 12 ? 0 : 1);
        }

        private void InitializePickers()
        {
            var periodList = (from SchedulePeriod suit in Enum.GetValues(typeof (SchedulePeriod)) select suit.Humanize()).ToList();

            _reminderPeriodSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, periodList);

            var hourChoices = Enumerable.Range(1, 12).ToList();
            var minuteChoices = new List<string> {"00", "30"};
            var meridianChoices = new List<string> {"AM", "PM"};


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

            var startHour = _startHourSpinner.SelectedItemPosition == 11 ? 0 : _startHourSpinner.SelectedItemPosition + 1;
            var startTime = new DateTime(1, 1, 1, startHour + (12 * _startMeridianSpinner.SelectedItemPosition), _startMinuteSpinner.SelectedItemPosition * 30, 0);

            var endHour = _endHourSpinner.SelectedItemPosition == 11 ? 0 : _endHourSpinner.SelectedItemPosition + 1;
            var endTime = new DateTime(1, 1, 1, endHour + (12 * _endMeridianSpinner.SelectedItemPosition), _endMinuteSpinner.SelectedItemPosition * 30, 0);

            if (startTime >= endTime)
            {
                _userNotification.ShowValidationErrorPopUp(this, "Please make sure your start time is before your end time");
                return;
            }

            _exerciseSchedule.StartTime = startTime;
            _exerciseSchedule.EndTime = endTime;

            _data.SaveExerciseSchedule(_exerciseSchedule);
            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);

            Finish();
        }
    }
}