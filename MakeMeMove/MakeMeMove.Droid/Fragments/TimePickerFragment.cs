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

namespace MakeMeMove.Droid.Fragments
{
    public class TimePickerFragment : Android.Support.V4.App.DialogFragment
    {
        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new QuarterHourTimePickerDialog(Activity, null, 8, 15, true);
        }


        private class QuarterHourTimePickerDialog : TimePickerDialog
        {
            private const int MINUTE_INTERVAL = 15;

            public QuarterHourTimePickerDialog(Context context, IOnTimeSetListener listener, int hourOfDay, int minute, bool is24HourView) 
                : base(context, Resource.Style.Theme_AppCompat_Light_Dialog, (EventHandler<TimeSetEventArgs>)null, hourOfDay, minute/* / MINUTE_INTERVAL*/, is24HourView)
            {
            }
        }
    }
}