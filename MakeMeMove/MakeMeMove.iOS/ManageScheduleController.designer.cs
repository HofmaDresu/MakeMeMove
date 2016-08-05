// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MakeMeMove.iOS
{
    [Register ("ManageScheduleController")]
    partial class ManageScheduleController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }
        }
    }
}