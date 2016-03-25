using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid
{
    public static class GeneralDialogs
    {

        public static void ShowNetworkErrorDialog(Activity activity, bool goBackOnNegative = true)
        {
            if (activity.IsFinishing || activity.IsDestroyed)
                return;

            if (IsAirplaneModeOn(activity))
            {
                new AlertDialog.Builder(activity)
                    .SetMessage(Resource.String.network_error_airplane)
                    .SetNegativeButton(Resource.String.cancel, (s, args) =>
                    {
                        if (goBackOnNegative)
                        {
                            activity.Finish();
                        }
                    })
                    .SetPositiveButton(Resource.String.network_settings_label, (s, args) =>
                    {
                        var intent = new Intent(Settings.ActionAirplaneModeSettings);
                        activity.StartActivity(intent);
                    })
                    .SetCancelable(false)
                    .SetTitle(Resource.String.network_error_title).Create().Show();
            }
            else
            {
                new AlertDialog.Builder(activity)
                    .SetMessage(Resource.String.network_error_connection)
                    .SetNegativeButton(Resource.String.cancel, (s, args) =>
                    {
                        if (goBackOnNegative)
                        {
                            activity.Finish();
                        }
                    })
                    .SetPositiveButton(Resource.String.settings_label, (s, args) =>
                    {
                        var intent = new Intent(Settings.ActionSettings);
                        activity.StartActivity(intent);
                    })
                    .SetCancelable(false)
                    .SetTitle(Resource.String.network_error_title).Create().Show();
            }
        }

        private static bool IsAirplaneModeOn(Activity activity)
        {
            return Settings.Global.GetInt(activity.ContentResolver, Settings.Global.AirplaneModeOn, 0) != 0;
        }
    }
}