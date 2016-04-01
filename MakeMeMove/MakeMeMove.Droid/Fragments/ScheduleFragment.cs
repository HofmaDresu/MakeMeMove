using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Fragments
{
    public class ScheduleFragment : BaseMainFragment
    {
        private Data _data;
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private ExerciseSchedule _exerciseSchedule;
        private Button _startServiceButton;
        private Button _stopServiceButton;
        private TextView _startTimeText;
        private TextView _endTimeText;
        private TextView _reminderPeriodText;
        private Button _manageScheduleButton;

        public void Initialize(Data data)
        {
            _data = data;
            Title = "My Schedule";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Main_ExerciseSchedule, container, false);

            _startServiceButton = view.FindViewById<Button>(Resource.Id.StartServiceButton);
            _stopServiceButton = view.FindViewById<Button>(Resource.Id.StopServiceButton);
            _startTimeText = view.FindViewById<TextView>(Resource.Id.StartTimeText);
            _endTimeText = view.FindViewById<TextView>(Resource.Id.EndTimeText);
            _reminderPeriodText = view.FindViewById<TextView>(Resource.Id.ReminderPeriodText);
            _manageScheduleButton = view.FindViewById<Button>(Resource.Id.ManageScheduleButton);

            _manageScheduleButton.Click += (o, e) => StartActivity(new Intent(Activity, typeof(ManageScheduleActivity)));
            _startServiceButton.Click += (o, e) => StartService();
            _stopServiceButton.Click += (o, e) => StopService();
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            _exerciseSchedule = _data.GetExerciseSchedule();
            _startTimeText.Text = _exerciseSchedule.StartTime.ToLongTimeString();
            _endTimeText.Text = _exerciseSchedule.EndTime.ToLongTimeString();
            _reminderPeriodText.Text = _exerciseSchedule.PeriodDisplayString;

            EnableDisableServiceButtons();
        }

        private void EnableDisableServiceButtons()
        {
            _startServiceButton.Enabled = !_serviceManager.NotificationServiceIsRunning(Activity);
            _stopServiceButton.Enabled = _serviceManager.NotificationServiceIsRunning(Activity);
        }

        private void StartService()
        {
            _serviceManager.StartNotificationService(Activity, _exerciseSchedule);
            EnableDisableServiceButtons();
        }

        private void StopService()
        {
            _serviceManager.StopNotificationService(Activity, _exerciseSchedule);
            EnableDisableServiceButtons();
        }
    }
}