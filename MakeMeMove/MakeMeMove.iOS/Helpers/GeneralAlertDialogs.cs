using System;
using UIKit;

namespace MakeMeMove.iOS.Helpers
{
	public static class GeneralAlertDialogs
	{
		public static void ShowValidationErrorPopUp(UIViewController controller, string validationMessage)
		{
			var okayAlertController = UIAlertController.Create("Invalid Information", validationMessage, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

			controller.PresentViewController(okayAlertController, true, null);

		}

	    public static void ShowNetworkErrorDialog(UIViewController controller, Action<UIAlertAction> okAction)
	    {
            var okayAlertController = UIAlertController.Create("Network Unavailable", "Network access is required and appears unavailable. Please try again later.", UIAlertControllerStyle.Alert);
            okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, okAction));
        }
	}
}

