using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Java.Lang;
using MakeMeMove.Model;
using Newtonsoft.Json;

namespace MakeMeMove.Droid
{
	[Activity (Label = "Make Me Move", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
	    private ExerciseSchedule _exerciseSchedule;
	    private Button _startButton;
	    private Button _stopButton;
	    private Button _editScheduleButton;
	    private Button _addExerciseButton;
	    private TextView _startTime;
	    private TextView _endTime;
	    private TextView _reminderPattern;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		    _startButton = FindViewById<Button>(Resource.Id.startServiceButton);
            _stopButton = FindViewById<Button>(Resource.Id.stopServiceButton);
            _editScheduleButton = FindViewById<Button>(Resource.Id.editScheduleButton);
            _addExerciseButton = FindViewById<Button>(Resource.Id.addExerciseButton);
		    _startTime = FindViewById<TextView>(Resource.Id.startTime);
            _endTime = FindViewById<TextView>(Resource.Id.endTime);
            _reminderPattern = FindViewById<TextView>(Resource.Id.reminderPattern);

            SetUpSchedule();

		}

	    private void SetUpSchedule()
	    {
	        var settings = GetSharedPreferences("MakeMeMove", FileCreationMode.Private);
	        var exercises = settings.GetString("Exercises", string.Empty);
	        if (!string.IsNullOrWhiteSpace(exercises))
	        {
	            _exerciseSchedule = JsonConvert.DeserializeObject<ExerciseSchedule>(exercises);
	        }
	        else
	        {
	            _exerciseSchedule = ExerciseSchedule.CreateDefaultSchedule();
	            var editor = settings.Edit();
	            editor.PutString("Exercises", JsonConvert.SerializeObject(_exerciseSchedule));
	            editor.Commit();
	        }

	        _startTime.Text = _exerciseSchedule.StartTime.ToShortTimeString();
            _endTime.Text = _exerciseSchedule.EndTime.ToShortTimeString();

	        switch (_exerciseSchedule.Period)
            {
                case SchedulePeriod.HalfHourly:
	                _reminderPattern.Text = "Every Half Hour";
	                break;
	            case SchedulePeriod.Hourly:
                    _reminderPattern.Text = "Every Hour";
                    break;
	            case SchedulePeriod.BiHourly:
                    _reminderPattern.Text = "Every Two Hours";
                    break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }
        
        protected override void OnPause()
        {
            base.OnPause();
            _startButton.Click -= StartButtonOnClick;
            _stopButton.Click -= StopButtonOnClick;
        }

	    protected override void OnResume()
	    {
	        base.OnResume();
            _startButton.Click += StartButtonOnClick;
            _stopButton.Click += StopButtonOnClick;
        }

	    private void StopButtonOnClick(object sender, EventArgs eventArgs)
        {
            var reminder = new Intent(this, typeof(ExerciseTickBroadcastReceiver));
            var recurringReminders = PendingIntent.GetBroadcast(this, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)GetSystemService(AlarmService);

            alarms.Cancel(recurringReminders);

            Toast.MakeText(this, "Service Stopped", ToastLength.Long).Show();
        }

	    private void StartButtonOnClick(object sender, EventArgs eventArgs)
	    {
            var reminder = new Intent(this, typeof(ExerciseTickBroadcastReceiver));
	        reminder.PutExtra("ExerciseSchedule", JsonConvert.SerializeObject(_exerciseSchedule));

            var recurringReminders = PendingIntent.GetBroadcast(this, 0, reminder, PendingIntentFlags.CancelCurrent);
            var alarms = (AlarmManager)GetSystemService(AlarmService);

            var nextRunTime = TickUtility.GetNextRunTime(_exerciseSchedule);
	        Log.Error("asdf", nextRunTime.ToShortDateString() + " " + nextRunTime.ToShortTimeString());
	        var dtBasis = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if ((int)Build.VERSION.SdkInt >= 19)
            {
                alarms.SetWindow(AlarmType.RtcWakeup,
                    (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds,
                    10 * 60 * 1000, recurringReminders);
            }
            else
            {
                alarms.Set(AlarmType.RtcWakeup,
                    (long)nextRunTime.ToUniversalTime().Subtract(dtBasis).TotalMilliseconds, recurringReminders);
            }

            Toast.MakeText(this, "Service Started", ToastLength.Long).Show();
        }
	}
}


