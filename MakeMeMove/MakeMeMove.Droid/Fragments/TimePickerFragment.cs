using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.App;
using static Android.App.TimePickerDialog;

namespace MakeMeMove.Droid.Fragments
{
    public class TimePickerFragment : Android.Support.V4.App.DialogFragment
    {
        private int _startHour;
        private int _startMinute;

        public TimePickerFragment() { }

        public TimePickerFragment(int startHour, int startMinute)
        {
            _startHour = startHour;
            _startMinute = startMinute;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new TimePickerDialog(Activity, Activity as IOnTimeSetListener, _startHour, _startMinute, true);
        }
    }
}