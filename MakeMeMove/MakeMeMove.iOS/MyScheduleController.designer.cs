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
    [Register ("MyScheduleController")]
    partial class MyScheduleController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EndTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ReminderPeriod { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StartTime { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch StatusSwitch { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (EndTime != null) {
                EndTime.Dispose ();
                EndTime = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }

            if (ReminderPeriod != null) {
                ReminderPeriod.Dispose ();
                ReminderPeriod = null;
            }

            if (StartTime != null) {
                StartTime.Dispose ();
                StartTime = null;
            }

            if (StatusSwitch != null) {
                StatusSwitch.Dispose ();
                StatusSwitch = null;
            }
        }
    }
}