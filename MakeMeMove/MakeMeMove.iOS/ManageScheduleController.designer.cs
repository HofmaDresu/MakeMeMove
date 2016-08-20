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
        MakeMeMove.iOS.PickerUITextField EndTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField StartTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EndTime != null) {
                EndTime.Dispose ();
                EndTime = null;
            }

            if (StartTime != null) {
                StartTime.Dispose ();
                StartTime = null;
            }
        }
    }
}