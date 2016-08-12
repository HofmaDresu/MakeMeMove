using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace MakeMeMove.iOS
{
	partial class PickerUITextField : UITextField
	{
		public PickerUITextField(IntPtr handle) : base(handle)
		{
		}

		public override bool CanPerform(Selector action, NSObject withSender)
		{
			return false;
		}
	}
}