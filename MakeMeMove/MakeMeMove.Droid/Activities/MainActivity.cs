using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private readonly ISchedulePersistence _schedulePersistence = new SchedulePersistence();
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private ExerciseSchedule _exerciseSchedule;
        private Button _startServiceButton;
        private Button _stopServiceButton;
        private TextView _startTimeText;
        private TextView _endTimeText;
        private TextView _reminderPeriodText;
        private ListView _exerciseListView;
        private Button _manageScheduleButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _startServiceButton = FindViewById<Button>(Resource.Id.StartServiceButton);
            _stopServiceButton = FindViewById<Button>(Resource.Id.StopServiceButton);
            _startTimeText = FindViewById<TextView>(Resource.Id.StartTimeText);
            _endTimeText = FindViewById<TextView>(Resource.Id.EndTimeText);
            _reminderPeriodText = FindViewById<TextView>(Resource.Id.ReminderPeriodText);
            _exerciseListView = FindViewById<ListView>(Resource.Id.ExerciseList);
            _manageScheduleButton = FindViewById<Button>(Resource.Id.ManageScheduleButton);

            _startServiceButton.Click += (o, e) => StartService();
            _stopServiceButton.Click += (o, e) => StopService();
            _manageScheduleButton.Click += (o, e) => StartActivity(new Intent(this, typeof (ManageScheduleActivity)));
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (!_schedulePersistence.HasExerciseSchedule())
            {
                _exerciseSchedule = ExerciseSchedule.CreateDefaultSchedule();
                _schedulePersistence.SaveExerciseSchedule(_exerciseSchedule);
            }
            else
            {
                _exerciseSchedule = _schedulePersistence.LoadExerciseSchedule();
            }

            _startTimeText.Text = _exerciseSchedule.StartTime.ToLongTimeString();
            _endTimeText.Text = _exerciseSchedule.EndTime.ToLongTimeString();
            _reminderPeriodText.Text = _exerciseSchedule.PeriodDisplayString;
            UpdateExerciseList();


            EnableDisableServiceButtons();
        }

        protected override void OnPause()
        {
            base.OnPause();
            _exerciseListView.Adapter?.Dispose();
        }

        private void EditExerciseClicked(object sender, Guid guid)
        {
            var selectedExercise = _exerciseSchedule.Exercises.FirstOrDefault(e => e.Id == guid);
            throw new NotImplementedException();
        }

        private void DeleteExerciseClicked(object sender, Guid guid)
        {
            var selectedExercise = _exerciseSchedule.Exercises.FirstOrDefault(e => e.Id == guid);
            if (selectedExercise != null)
            {
                _exerciseSchedule.Exercises.Remove(selectedExercise);
            }
            _schedulePersistence.SaveExerciseSchedule(_exerciseSchedule);
            UpdateExerciseList();
        }

        private void UpdateExerciseList()
        {
            var exerciseListAdapter = new ExerciseListAdapter(_exerciseSchedule.Exercises, this);
            exerciseListAdapter.DeleteExerciseClicked += DeleteExerciseClicked;
            exerciseListAdapter.EditExerciseClicked += EditExerciseClicked;
            _exerciseListView.Adapter = exerciseListAdapter;
            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);
        }

        private void EnableDisableServiceButtons()
        {
            _startServiceButton.Enabled = !_serviceManager.NotificationServiceIsRunning(this);
            _stopServiceButton.Enabled = _serviceManager.NotificationServiceIsRunning(this);
        }

        private void StartService()
        {
            _serviceManager.StartNotificationService(this, _exerciseSchedule);
            EnableDisableServiceButtons();
        }

        private void StopService()
        {
            _serviceManager.StopNotificationService(this, _exerciseSchedule);
            EnableDisableServiceButtons();
        }
    }
}

