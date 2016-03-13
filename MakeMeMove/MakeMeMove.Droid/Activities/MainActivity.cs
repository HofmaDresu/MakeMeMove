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
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using MacroEatMobile.Core;
using MakeMeMove.Droid.Utilities;

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
        private DrawerLayout _drawer;
        private Person _person;

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
            _drawer = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

            _startServiceButton.Click += (o, e) => StartService();
            _stopServiceButton.Click += (o, e) => StopService();
            _manageScheduleButton.Click += (o, e) => StartActivity(new Intent(this, typeof (ManageScheduleActivity)));
            _addExerciseButton.Click += (sender, args) => StartActivity(new Intent(this, typeof(ManageExerciseActivity)));

            FindViewById(Resource.Id.ViewHistoryButton).Click += (sender, args) =>
            {
                _drawer.CloseDrawer(GravityCompat.Start);
                if (AuthorizationSingleton.PersonIsProOrHigherUser(_person))
                {
                    StartActivity(new Intent(this, typeof(ExerciseHistoryActivity)));
                }
                else if(_person == null)
                {
                    new AlertDialog.Builder(this)
                        .SetTitle("Account Needed")
                        .SetMessage("You must sign in as a Fudist Premium user to access your exercise history. Would you like to sign in?")
                        .SetPositiveButton("Yes", (o, eventArgs) => { }) 
                        .SetNegativeButton("No", (o, eventArgs) => { })
                        .Show();
                }
                else
                {
                    new AlertDialog.Builder(this)
                        .SetTitle("Premium Account Needed")
                        .SetMessage("Your current account is not subscribed to Fudist Premium. Please double check your subscription status and try again.")
                        .SetPositiveButton("OK", (o, eventArgs) => { })
                        .Show();
                }
            };

            _permissionRequester.RequestPermissions(this);

            _exerciseRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
        }

        protected override async void OnResume()
        {
            base.OnResume();
            _person = await AuthorizationSingleton.GetInstance().GetPerson(this, true);

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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Settings, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem selectedItem)
        {
            if (_drawer.IsDrawerOpen(GravityCompat.Start))
            {
                _drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                _drawer.OpenDrawer(GravityCompat.Start);
            }
            return true;
        }
    }
}

