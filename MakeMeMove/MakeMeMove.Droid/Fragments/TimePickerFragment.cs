using Android.OS;
using Android.App;
using static Android.App.TimePickerDialog;
using Android.Text.Format;

namespace MakeMeMove.Droid.Fragments
{
    public class TimePickerFragment : Android.Support.V4.App.DialogFragment
    {
        private readonly int _startHour;
        private readonly int _startMinute;

        public TimePickerFragment() { }

        public TimePickerFragment(int startHour, int startMinute)
        {
            _startHour = startHour;
            _startMinute = startMinute;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            return new TimePickerDialog(Activity, Activity as IOnTimeSetListener, _startHour, _startMinute, DateFormat.Is24HourFormat(Activity));
        }
    }
}