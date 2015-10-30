using System;
using System.Collections.Generic;
using System.Text;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.iOS.DeviceSpecificImplementations;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(UserNotification))]
namespace MakeMeMove.iOS.DeviceSpecificImplementations
{
    public class UserNotification : IUserNotification
    {
        public void ShowValidationErrorPopUp(string message, Action onCloseAction = null)
        {
            var accountNeededAlert = new UIAlertView("Invalid Information", message,
                        null, "Ok");

            accountNeededAlert.Clicked += (sender, args) =>
            {
                onCloseAction?.Invoke();
            };

            accountNeededAlert.Show();
        }
    }
}
