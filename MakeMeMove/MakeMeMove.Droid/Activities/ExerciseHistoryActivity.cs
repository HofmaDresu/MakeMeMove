using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Exercise History", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ExerciseHistoryActivity : BaseActivity
    {
        private TextView _date;
        private ListView _stats;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExerciseHistory);


            _date = FindViewById<TextView>(Resource.Id.Date);
            _stats = FindViewById<ListView>(Resource.Id.Stats);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var todaysStats = Data.GetExerciseHistoryForDay(DateTime.Today);

            _date.Text = DateTime.Today.ToShortDateString();

            _stats.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, todaysStats.Select(s => $"{s.ExerciseName}: {s.QuantityCompleted} / {s.QuantityNotified}").ToList());
        }
    }
}