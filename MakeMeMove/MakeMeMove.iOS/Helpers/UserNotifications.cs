using System;
using UIKit;

namespace MakeMeMove.iOS.Helpers
{
	public static class UserNotifications
	{
		public static void ShowValidationErrorPopUp(UIViewController controller, string validationMessage)
		{
			UIAlertController okayAlertController = UIAlertController.Create("Invalid Information", validationMessage, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			controller.PresentViewController(okayAlertController, true, null);

		}
	}
}

