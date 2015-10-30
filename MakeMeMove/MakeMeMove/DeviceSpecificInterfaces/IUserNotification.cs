using System;

namespace MakeMeMove.DeviceSpecificInterfaces
{
    public interface IUserNotification
    {
        void ShowValidationErrorPopUp(string message, Action onCloseAction = null);
    }
}
