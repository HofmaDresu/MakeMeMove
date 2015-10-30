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

        public void ShowAreYouSureDialog(string message, Action onYesAction = null, Action onNoAction = null)
        {
            var accountNeededAlert = new UIAlertView("Are you sure?", message,
                        null, "Ok", "Cancel");

            accountNeededAlert.Clicked += (sender, args) =>
            {
                switch (args.ButtonIndex)
                {
                    case 0:
                        onYesAction?.Invoke();
                        break;
                    case 1:
                        onNoAction?.Invoke();
                        break;
                }
            };

            accountNeededAlert.Show();
        }
    }
}
