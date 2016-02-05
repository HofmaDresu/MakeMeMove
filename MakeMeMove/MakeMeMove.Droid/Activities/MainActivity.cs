using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Widget;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using System.Collections.Generic;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity
    {
        private readonly ExerciseServiceManager _serviceManager = new ExerciseServiceManager();
        private readonly PermissionRequester _permissionRequester = new PermissionRequester();
        private ExerciseSchedule _exerciseSchedule;
        private List<ExerciseBlock> _exerciseBlocks;
        private Button _startServiceButton;
        private Button _stopServiceButton;
        private TextView _startTimeText;
        private TextView _endTimeText;
        private TextView _reminderPeriodText;
        private RecyclerView _exerciseRecyclerView;
        private Button _manageScheduleButton;
        private Button _addExerciseButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _startServiceButton = FindViewById<Button>(Resource.Id.StartServiceButton);
            _stopServiceButton = FindViewById<Button>(Resource.Id.StopServiceButton);
            _startTimeText = FindViewById<TextView>(Resource.Id.StartTimeText);
            _endTimeText = FindViewById<TextView>(Resource.Id.EndTimeText);
            _reminderPeriodText = FindViewById<TextView>(Resource.Id.ReminderPeriodText);
            _exerciseRecyclerView = FindViewById<RecyclerView>(Resource.Id.ExerciseList);
            _manageScheduleButton = FindViewById<Button>(Resource.Id.ManageScheduleButton);
            _addExerciseButton = FindViewById<Button>(Resource.Id.AddExerciseButton);

            _startServiceButton.Click += (o, e) => StartService();
            _stopServiceButton.Click += (o, e) => StopService();
            _manageScheduleButton.Click += (o, e) => StartActivity(new Intent(this, typeof (ManageScheduleActivity)));
            _addExerciseButton.Click += (sender, args) => StartActivity(new Intent(this, typeof(ManageExerciseActivity)));

            FindViewById<Button>(Resource.Id.ViewHistoryButton).Click += (sender, args) => StartActivity(new Intent(this, typeof(ExerciseHistory)));

            _permissionRequester.RequestPermissions(this);

            _exerciseRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
        }

        protected override void OnResume()
        {
            base.OnResume();

            _exerciseSchedule = Data.GetExerciseSchedule();
            _exerciseBlocks = Data.GetExerciseBlocks();

            _startTimeText.Text = _exerciseSchedule.StartTime.ToLongTimeString();
            _endTimeText.Text = _exerciseSchedule.EndTime.ToLongTimeString();
            _reminderPeriodText.Text = _exerciseSchedule.PeriodDisplayString;
            UpdateExerciseList();


            EnableDisableServiceButtons();
        }

        private void EditExerciseClicked(object sender, int id)
        {
            var intent = new Intent(this, typeof (ManageExerciseActivity));
            intent.PutExtra(Constants.ExerciseId, id);
            StartActivity(intent);
        }

        private void DeleteExerciseClicked(object sender, int id)
        {
            var selectedExercise = _exerciseBlocks.FirstOrDefault(e => e.Id == id);
            if (selectedExercise != null)
            {
                var exerciseIndex = _exerciseBlocks.IndexOf(selectedExercise);
                Data.DeleteExerciseBlock(selectedExercise.Id);
                _exerciseBlocks = Data.GetExerciseBlocks();

                var adapter =(ExerciseRecyclerAdapter) _exerciseRecyclerView.GetAdapter();
                adapter.UpdateExerciseList(_exerciseBlocks);
                adapter.NotifyItemRemoved(exerciseIndex);
            }
        }

        private void UpdateExerciseList()
        {
            var exerciseListAdapter = new ExerciseRecyclerAdapter(_exerciseBlocks);
            exerciseListAdapter.DeleteExerciseClicked += DeleteExerciseClicked;
            exerciseListAdapter.EditExerciseClicked += EditExerciseClicked;
            exerciseListAdapter.EnableDisableClicked += EnableDisableClicked;
            _exerciseRecyclerView.SetAdapter(exerciseListAdapter);
            _serviceManager.RestartNotificationServiceIfNeeded(this, _exerciseSchedule);
        }

        private void EnableDisableClicked(object sender, int id)
        {
            var selectedExercise = _exerciseBlocks.FirstOrDefault(e => e.Id == id);
            if (selectedExercise != null)
            {
                selectedExercise.Enabled = !selectedExercise.Enabled;
                Data.UpdateExerciseBlock(selectedExercise);
                _exerciseBlocks = Data.GetExerciseBlocks();
            }
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

