using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Humanizer;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "ManageScheduleActivitiy")]
    public class ManageScheduleActivity : Activity
    {
        private Spinner _reminderPeriodSpinner;
        private Spinner _startHourSpinner;
        private Spinner _startMinuteSpinner;
        private Spinner _startMeridianSpinner;
        private Spinner _endHourSpinner;
        private Spinner _endMinuteSpinner;
        private Spinner _endMeridianSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            SetContentView(Resource.Layout.ManageSchedule);
            _reminderPeriodSpinner = FindViewById<Spinner>(Resource.Id.ReminderSpinner);
            _startHourSpinner = FindViewById<Spinner>(Resource.Id.StartHourSpinner);
            _startMinuteSpinner = FindViewById<Spinner>(Resource.Id.StartMinuteSpinner);
            _startMeridianSpinner = FindViewById<Spinner>(Resource.Id.StartMeridianSpinner);
            _endHourSpinner = FindViewById<Spinner>(Resource.Id.EndHourSpinner);
            _endMinuteSpinner = FindViewById<Spinner>(Resource.Id.EndMinuteSpinner);
            _endMeridianSpinner = FindViewById<Spinner>(Resource.Id.EndMeridianSpinner);


            InitializePickers();
        }



        private void InitializePickers()
        {
            var periodList = (from SchedulePeriod suit in Enum.GetValues(typeof (SchedulePeriod)) select suit.Humanize()).ToList();

            _reminderPeriodSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, periodList);

            var hourChoices = Enumerable.Range(1, 12).ToList();
            var minuteChoices = new List<string> {"00", "30"};
            var meridianChoices = new List<string> {"AM", "PM"};


            _startHourSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, hourChoices);
            _startMinuteSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, minuteChoices);
            _startMeridianSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, meridianChoices);
            _endHourSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, hourChoices);
            _endMinuteSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, minuteChoices);
            _endMeridianSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, meridianChoices);
        }
    }
}