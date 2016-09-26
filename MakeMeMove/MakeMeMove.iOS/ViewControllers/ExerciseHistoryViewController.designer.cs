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

namespace MakeMeMove.iOS.ViewControllers
{
    [Register ("ExerciseHistoryViewController")]
    partial class ExerciseHistoryViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView DateDisplayView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ExerciseHistoryContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DateDisplayView != null) {
                DateDisplayView.Dispose ();
                DateDisplayView = null;
            }

            if (ExerciseHistoryContainer != null) {
                ExerciseHistoryContainer.Dispose ();
                ExerciseHistoryContainer = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }
        }
    }
}