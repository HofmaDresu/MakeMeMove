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
using Android.Preferences;
using Android.Media;

namespace MakeMeMove.Droid.Fragments
{
    public class SettingsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        private string _notificationSoundKey;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _notificationSoundKey = Resources.GetString(Resource.String.NotificationSoundKey);

            AddPreferencesFromResource(Resource.Xml.preferences);

            SetNotificationSoundSummary(PreferenceManager.GetDefaultSharedPreferences(Activity));
        }

        public override void OnStart()
        {
            base.OnStart();
            PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnStop()
        {
            base.OnStop();
            PreferenceManager.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            if (key == _notificationSoundKey)
            {
                SetNotificationSoundSummary(sharedPreferences);
            }
        }

        private void SetNotificationSoundSummary(ISharedPreferences sharedPreferences)
        {
            Preference exercisesPref = FindPreference(_notificationSoundKey);
            exercisesPref.Summary = RingtoneManager.GetRingtone(Activity, Android.Net.Uri.Parse(sharedPreferences.GetString(_notificationSoundKey, ""))).GetTitle(Activity);
        }
    }
}