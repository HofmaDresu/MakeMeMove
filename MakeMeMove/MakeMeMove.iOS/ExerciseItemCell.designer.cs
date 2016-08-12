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
    [Register ("ExerciseItemCell")]
    partial class ExerciseItemCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch ExerciseIsEnabled { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ExerciseNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ExerciseIsEnabled != null) {
                ExerciseIsEnabled.Dispose ();
                ExerciseIsEnabled = null;
            }

            if (ExerciseNameLabel != null) {
                ExerciseNameLabel.Dispose ();
                ExerciseNameLabel = null;
            }
        }
    }
}