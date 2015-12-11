using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;

namespace MakeMeMove.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private readonly ISchedulePersistence _schedulePersistence = new SchedulePersistence();
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private ExerciseSchedule _exerciseSchedule;
        private Button _startServiceButton;
        private Button _stopServiceButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _startServiceButton = FindViewById<Button>(Resource.Id.StartServiceButton);
            _stopServiceButton = FindViewById<Button>(Resource.Id.StopServiceButton);


            _startServiceButton.Click += (o, e) => StartService();
            _stopServiceButton.Click += (o, e) => StopService();
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

            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);

            EnableDisableServiceButtons();
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

