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
    [Register ("MyExercisesController")]
    partial class MyExercisesController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView ExerciseList { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UINavigationBar NavBar { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ExerciseList != null) {
                ExerciseList.Dispose ();
                ExerciseList = null;
            }

            if (NavBar != null) {
                NavBar.Dispose ();
                NavBar = null;
            }
        }
    }
}