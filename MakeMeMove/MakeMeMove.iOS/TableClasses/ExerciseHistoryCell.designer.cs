// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace MakeMeMove.iOS.TableClasses
{
    [Register ("ExerciseHistoryCell")]
    partial class ExerciseHistoryCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ExerciseCountLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ExerciseName { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ExerciseCountLabel != null) {
                ExerciseCountLabel.Dispose ();
                ExerciseCountLabel = null;
            }

            if (ExerciseName != null) {
                ExerciseName.Dispose ();
                ExerciseName = null;
            }
        }
    }
}