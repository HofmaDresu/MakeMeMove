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
using Android.Media;
using Android.Support.V7.Preferences;
using Android.Provider;

namespace MakeMeMove.Droid.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        private string _notificationSoundKey;
        private ISharedPreferences _sharedPreferences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _notificationSoundKey = Resources.GetString(Resource.String.NotificationSoundKey);

            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity);

            AddPreferencesFromResource(Resource.Xml.preferences);

            SetNotificationSoundSummary();
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
        }

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            if (preference.Key.Equals(_notificationSoundKey))
            {
                var intent = new Intent(RingtoneManager.ActionRingtonePicker);
                intent.PutExtra(RingtoneManager.ExtraRingtoneType, (int)RingtoneType.Notification);
                intent.PutExtra(RingtoneManager.ExtraRingtoneShowDefault, true);
                intent.PutExtra(RingtoneManager.ExtraRingtoneShowSilent, true);
                intent.PutExtra(RingtoneManager.ExtraRingtoneDefaultUri, Settings.System.DefaultNotificationUri);

                var currentRingtone = _sharedPreferences.GetString(_notificationSoundKey, null);
                if (currentRingtone == null)
                {
                    intent.PutExtra(RingtoneManager.ExtraRingtoneExistingUri, Settings.System.DefaultNotificationUri);
                }
                else
                {
                    intent.PutExtra(RingtoneManager.ExtraRingtoneExistingUri, Android.Net.Uri.Parse(currentRingtone));
                }

                StartActivityForResult(intent, Constants.NotificationSoundRequestCode);
                return true;
            }

            return base.OnPreferenceTreeClick(preference);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == Constants.NotificationSoundRequestCode && data != null)
            {
                var ringtone = (Android.Net.Uri)data.GetParcelableExtra(RingtoneManager.ExtraRingtonePickedUri);
                var editor = _sharedPreferences.Edit();
                editor.PutString(_notificationSoundKey, ringtone?.ToString());
                editor.Apply();
                SetNotificationSoundSummary(ringtone);
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        private void SetNotificationSoundSummary()
        {
            SetNotificationSoundSummary(Android.Net.Uri.Parse(_sharedPreferences.GetString(_notificationSoundKey, "")));
        }

        private void SetNotificationSoundSummary(Android.Net.Uri ringtoneUri)
        {
            Preference exercisesPref = FindPreference(_notificationSoundKey);
            exercisesPref.Summary = RingtoneManager.GetRingtone(Activity, ringtoneUri).GetTitle(Activity);
        }
    }
}