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
using Humanizer;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "ManageExerciseActivity", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageExerciseActivity : Activity
    {
        private Spinner _exerciseTypeSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ManageExercise);

            _exerciseTypeSpinner = FindViewById<Spinner>(Resource.Id.ExerciseTypeSpinner);

            InitializePickers();
        }

        private void InitializePickers()
        {
            var exerciseList = (from PreBuiltExersises suit in Enum.GetValues(typeof(PreBuiltExersises)) select suit.Humanize()).ToList();

            _exerciseTypeSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, exerciseList);
        }
    }
}