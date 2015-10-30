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
    public class UserNotification : IUserNotification
    {
        public void ShowValidationErrorPopUp(string message, Action onCloseAction = null)
        {
            new AlertDialog.Builder(Forms.Context)
                .SetTitle("Invalid Information")
                .SetMessage(message)
                .SetCancelable(false)
                .SetPositiveButton("OK", (sender, args) => { onCloseAction?.Invoke();})
                .Show();
        }
    }
}