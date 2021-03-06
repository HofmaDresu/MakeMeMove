using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Droid.Dialogs;
using MakeMeMove.Droid.Fragments;
using MakeMeMove.Model;
using MakeMeMove.Standard.Model;
using Xamarin.Essentials;
using static Android.App.TimePickerDialog;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Manage Schedule", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageScheduleActivity : BaseActivity, IOnTimeSetListener, IAddLocationDialogListener
    {
        private const string SELECTED_PICKER_BUNDLE_KEY = "selected_picker";
        private Spinner _scheduleTypeSpinner;
        private Spinner _reminderPeriodSpinner;
        private Button _saveButton;
        private Button _cancelButton;
        private View _startTimeContainer;
        private TextView _startTimeText;
        private View _endTimeContainer;
        private TextView _endTimeText;
        private CheckBox _movementLocationsEnabledCheckbox;
        private View _movementLocationsContainer;
        private RecyclerView _movementLocationRecyclerView;
        private View _loadingIndicator;

        private List<MovementLocation> _initialMovementLocations;
        private List<MovementLocation> _updatedMovementLocations;

        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private readonly UserNotification _userNotification = new UserNotification();
        private ExerciseSchedule _exerciseSchedule;

        private enum TimePickerType
        {
            None, Start, End
        }

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
            _movementLocationsEnabledCheckbox = FindViewById<CheckBox>(Resource.Id.MovementLocationsEnabledCheckbox);
            _movementLocationsContainer = FindViewById(Resource.Id.MovementLocationsContainer);
            _movementLocationRecyclerView = FindViewById<RecyclerView>(Resource.Id.MovementLocationsList);
            _loadingIndicator = FindViewById(Resource.Id.LoadingIndicator);


            InitializePickers();

            SetData();

            _cancelButton.Click += (s, e) => Finish();
            _saveButton.Click += (s, e) => SaveData();

            _movementLocationsEnabledCheckbox.CheckedChange += MovementLocationsEnabledCheckbox_CheckedChange;
        }

        private void SetData()
        {
            _reminderPeriodSpinner.SetSelection((int) _exerciseSchedule.Period);
            _scheduleTypeSpinner.SetSelection((int)_exerciseSchedule.Type);
            _startTimeText.Text = _exerciseSchedule.StartTime.ToShortTimeString();
            _endTimeText.Text = _exerciseSchedule.EndTime.ToShortTimeString();
            _selectedStartTime = _exerciseSchedule.StartTime;
            _selectedEndTime = _exerciseSchedule.EndTime;
            var isMovementLocationsEnabled = Data.IsMovementLocationsEnabled();
            _movementLocationsEnabledCheckbox.Checked = isMovementLocationsEnabled; 
            SetMovementLocationsVisibility(isMovementLocationsEnabled);
            _initialMovementLocations = Data.GetMovementLocations();
            _updatedMovementLocations = Data.GetMovementLocations();

            var movementLocationAdapter = new MovementLocationRecyclerAdapter(_updatedMovementLocations);
            movementLocationAdapter.AddMovementLocationClicked += AddMovementLocationClicked;
            movementLocationAdapter.DeleteMovementLocationClicked += DeleteMovementLocationClicked;
            _movementLocationRecyclerView.SetAdapter(movementLocationAdapter);
            _movementLocationRecyclerView.HasFixedSize = false;
            _movementLocationRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
        }

        private void InitializePickers()
        {
			var periodList = PickerListHelper.GetExercisePeriods();
            var scheduleTypeList = PickerListHelper.GetScheduleTypes();

            _reminderPeriodSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, periodList);
            _scheduleTypeSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, scheduleTypeList);

            _startTimeContainer.Click += StartTimeContainer_Click;
            _endTimeContainer.Click += EndTimeContainer_Click;
        }

        private void StartTimeContainer_Click(object sender, EventArgs e)
        {
            _selectedPicker = TimePickerType.Start;
            new TimePickerFragment(_selectedStartTime.Hour, _selectedStartTime.Minute).Show(SupportFragmentManager, "StartTimePicker");
        }

        private void EndTimeContainer_Click(object sender, EventArgs e)
        {
            _selectedPicker = TimePickerType.End;
            new TimePickerFragment(_selectedEndTime.Hour, _selectedEndTime.Minute).Show(SupportFragmentManager, "EndTimePicker");
        }

        private async void MovementLocationsEnabledCheckbox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            SetMovementLocationsVisibility(e.IsChecked);

            if (e.IsChecked)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
            }
        }

        private void SetMovementLocationsVisibility(bool movementLocationsEnabled) => _movementLocationsContainer.Visibility = movementLocationsEnabled ? ViewStates.Visible : ViewStates.Gone;

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
            Data.SetMovementLocationsEnabled(_movementLocationsEnabledCheckbox.Checked);

            Data.InsertAllMovementLocations(_updatedMovementLocations.Except(_initialMovementLocations));
            Data.DeleteAllMovementLocations(_initialMovementLocations.Except(_updatedMovementLocations).Select(ml => ml.Id));

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

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(SELECTED_PICKER_BUNDLE_KEY, (int)_selectedPicker);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            _selectedPicker = (TimePickerType)savedInstanceState.GetInt(SELECTED_PICKER_BUNDLE_KEY);
        }

        private void AddMovementLocationClicked(object sender, object args)
        {
            new AddLocationDialog().Show(SupportFragmentManager, "AddMovementLocation");
        }

        public async void OnSaveClick(string locationName)
        {
            try
            {
                _loadingIndicator.Visibility = ViewStates.Visible;
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);
                var newId = !_updatedMovementLocations.Any() ? -1 : Math.Min(_updatedMovementLocations.Select(ml => ml.Id).Min() - 1, -1);
                _updatedMovementLocations.Add(new MovementLocation { Id = newId, Name = locationName, Latitude = location.Latitude, Longitude = location.Longitude });
                RunOnUiThread(() =>
                {
                    var adapter = (MovementLocationRecyclerAdapter)_movementLocationRecyclerView.GetAdapter();
                    adapter.UpdateMovementLocationList(_updatedMovementLocations);
                    adapter.NotifyItemInserted(_updatedMovementLocations.Count - 1);
                });
            }
            finally
            {
                _loadingIndicator.Visibility = ViewStates.Gone;
            }
        }

        private void DeleteMovementLocationClicked(object sender, int id)
        {
            var selectedMovementLocation = _updatedMovementLocations.FirstOrDefault(e => e.Id == id);
            if (selectedMovementLocation != null)
            {
                var movementLocationIndex = _updatedMovementLocations.IndexOf(selectedMovementLocation);
                _updatedMovementLocations.RemoveAt(movementLocationIndex);

                var adapter = (MovementLocationRecyclerAdapter)_movementLocationRecyclerView.GetAdapter();
                adapter.UpdateMovementLocationList(_updatedMovementLocations);
                adapter.NotifyItemRemoved(movementLocationIndex);
            }
        }
    }
}