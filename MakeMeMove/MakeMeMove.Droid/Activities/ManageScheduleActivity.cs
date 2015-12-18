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

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "ManageScheduleActivitiy")]
    public class ManageScheduleActivity : Activity
    {
        private EditText _startTimeText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.ManageSchedule);

            _startTimeText = FindViewById<EditText>(Resource.Id.StartTimeEdit);


            _startTimeText.Click += (o, e) => new TimePickerDialog(this, (target, args) => {  }, 8, 0, false).Show();
        }
    }
}