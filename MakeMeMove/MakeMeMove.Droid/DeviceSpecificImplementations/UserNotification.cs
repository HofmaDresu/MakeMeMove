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
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserNotification))]
namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class UserNotification
    {
        public void ShowValidationErrorPopUp(Context context, string message, Action onCloseAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle("Invalid Information")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("OK", (sender, args) => { onCloseAction?.Invoke();})
                .Show();
        }

        public void ShowAreYouSureDialog(Context context, string message, Action onYesAction = null, Action onNoAction = null)
        {
            new AlertDialog.Builder(context)
                .SetTitle("Are you sure?")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("Yes", (sender, args) => { onYesAction?.Invoke(); })
                .SetNegativeButton("No", (sender, args) => { onNoAction?.Invoke(); })
                .Show();
        }
    }
}