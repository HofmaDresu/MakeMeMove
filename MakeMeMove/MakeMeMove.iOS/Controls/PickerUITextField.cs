using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;
using MakeMeMove.iOS.Helpers;

namespace MakeMeMove.iOS
{
	public partial class PickerUITextField : UITextField
	{
		public PickerUITextField(IntPtr handle) : base(handle)
		{
			var foo = "▼";
			var arrowButton = new UIButton(new CGRect(0, 0, 20, 20));
			arrowButton.TranslatesAutoresizingMaskIntoConstraints = false;
			arrowButton.SetTitle(foo, UIControlState.Normal);
			arrowButton.SetTitleColor(Colors.InteractableTextColor, UIControlState.Normal);
			arrowButton.UserInteractionEnabled = false;
			AddSubview(arrowButton);

			arrowButton.RightAnchor.ConstraintGreaterThanOrEqualTo(this.RightAnchor, 0).Active = true;
			arrowButton.CenterYAnchor.ConstraintEqualTo(this.CenterYAnchor).Active = true;
		}

		public override bool CanPerform(Selector action, NSObject withSender)
		{
			return false;
		}
	}
}