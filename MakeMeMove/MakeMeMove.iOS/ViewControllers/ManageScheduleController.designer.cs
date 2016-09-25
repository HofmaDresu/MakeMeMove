// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;

namespace MakeMeMove.iOS.ViewControllers
{
    [Register ("ManageScheduleController")]
    partial class ManageScheduleController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField EndTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField ReminderPeriod { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField StartTime { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EndTime != null) {
                EndTime.Dispose ();
                EndTime = null;
            }

            if (ReminderPeriod != null) {
                ReminderPeriod.Dispose ();
                ReminderPeriod = null;
            }

            if (StartTime != null) {
                StartTime.Dispose ();
                StartTime = null;
            }
        }
    }
}