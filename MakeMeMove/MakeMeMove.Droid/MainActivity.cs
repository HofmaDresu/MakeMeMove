using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
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

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);
		    _startButton = FindViewById<Button>(Resource.Id.startServiceButton);
            _stopButton = FindViewById<Button>(Resource.Id.stopServiceButton);

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
            StopService(new Intent(this, typeof(ExerciseTickService)));
            Toast.MakeText(this, "Service Stopped", ToastLength.Long).Show();
        }

	    private void StartButtonOnClick(object sender, EventArgs eventArgs)
	    {
            StartService(new Intent(this, typeof(ExerciseTickService)));
            Toast.MakeText(this, "Service Started", ToastLength.Long).Show();
        }
	}
}


