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

namespace MakeMeMove.iOS.ViewControllers
{
    [Register ("ManageExerciseViewController")]
    partial class ManageExerciseViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField CustomExerciseName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint CustomTextHeightConstraint { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField ExerciseType { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MakeMeMove.iOS.PickerUITextField NumberOfRepetitions { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CustomExerciseName != null) {
                CustomExerciseName.Dispose ();
                CustomExerciseName = null;
            }

            if (CustomTextHeightConstraint != null) {
                CustomTextHeightConstraint.Dispose ();
                CustomTextHeightConstraint = null;
            }

            if (ExerciseType != null) {
                ExerciseType.Dispose ();
                ExerciseType = null;
            }

            if (NumberOfRepetitions != null) {
                NumberOfRepetitions.Dispose ();
                NumberOfRepetitions = null;
            }
        }
    }
}