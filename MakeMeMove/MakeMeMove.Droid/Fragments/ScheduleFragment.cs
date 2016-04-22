using System.IO;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.Activities;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using SQLite;
using Environment = System.Environment;

namespace MakeMeMove.Droid.Fragments
{
    public class ScheduleFragment : BaseMainFragment
    {
        private Data _data;
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private ExerciseSchedule _exerciseSchedule;
        private TextView _startTimeText;
        private TextView _endTimeText;
        private TextView _reminderPeriodText;
        private Button _manageScheduleButton;
        private View _serviceToggle;
        private TextView _serviceStopped;
        private TextView _serviceStarted;

        public void Initialize(string title)
        {
            Title = title;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _data = _data ?? Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));
            var view = inflater.Inflate(Resource.Layout.Main_ExerciseSchedule, container, false);
            
            _startTimeText = view.FindViewById<TextView>(Resource.Id.StartTimeText);
            _endTimeText = view.FindViewById<TextView>(Resource.Id.EndTimeText);
            _reminderPeriodText = view.FindViewById<TextView>(Resource.Id.ReminderPeriodText);
            _manageScheduleButton = view.FindViewById<Button>(Resource.Id.ManageScheduleButton);

            _serviceToggle = view.FindViewById(Resource.Id.ServiceToggle);
            _serviceStopped = view.FindViewById<TextView>(Resource.Id.ServiceStopped);
            _serviceStarted = view.FindViewById<TextView>(Resource.Id.ServiceStarted);

            _manageScheduleButton.Click += (o, e) => StartActivity(new Intent(Activity, typeof(ManageScheduleActivity)));

            _serviceToggle.Click += (sender, args) => ToggleService();
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
            if (_serviceManager.NotificationServiceIsRunning(Activity))
            {
                _serviceStarted.SetBackgroundResource(Resource.Drawable.CustomSwitchPositiveActive);
                _serviceStarted.SetText(Resource.String.SwitchStarted);

                _serviceStopped.SetBackgroundResource(Android.Resource.Color.Transparent);
                _serviceStopped.Text = "";
            }
            else
            {
                _serviceStarted.SetBackgroundResource(Android.Resource.Color.Transparent);
                _serviceStarted.Text = "";

                _serviceStopped.SetBackgroundResource(Resource.Drawable.CustomSwitchNegativeActive);
                _serviceStopped.SetText(Resource.String.SwitchStopped);
            }
        }

        private void ToggleService()
        {
            if (_serviceManager.NotificationServiceIsRunning(Activity))
            {
                _serviceManager.StopNotificationService(Activity, _exerciseSchedule);
            }
            else
            {
                _serviceManager.StartNotificationService(Activity, _exerciseSchedule);

            }

            EnableDisableServiceButtons();
        }
    }
}