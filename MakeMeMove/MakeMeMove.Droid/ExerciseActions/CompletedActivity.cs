using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.DeviceSpecificImplementations;

namespace MakeMeMove.Droid.ExerciseActions
{
    public class CompletedActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Log.Error("asdf", "savingThing");

            new ProgressPersistence().SaveCompletedExercise(DateTime.Now.Date, Intent.GetStringExtra(Constants.ExerciseName), Intent.GetIntExtra(Constants.ExerciseQuantity, 0));

            Finish();
        }
    }
}