using System;

namespace MakeMeMove.DeviceSpecificInterfaces
{
    public interface IUserNotification
    {
        void ShowValidationErrorPopUp(string message, Action onCloseAction = null);
        void ShowAreYouSureDialog(string message, Action onYesAction = null, Action onNoAction = null);
    }
}
