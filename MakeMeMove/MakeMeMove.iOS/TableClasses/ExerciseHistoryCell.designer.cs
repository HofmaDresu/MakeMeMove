// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//

using System.CodeDom.Compiler;
using Foundation;

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