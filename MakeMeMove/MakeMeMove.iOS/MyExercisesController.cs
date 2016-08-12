using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using MakeMeMove.Model;

namespace MakeMeMove.iOS
{
    public partial class MyExercisesController : BaseViewController
    {
		private ExerciseTableDelegate _exerciseTableDelegate;
		private List<ExerciseBlock> _exercises;
		private const string ManageExerciseSegueId = "ManageExerciseSegue";
		public int? SelectedExerciseId;

        public MyExercisesController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			SelectedExerciseId = null;

			_exercises = Data.GetExerciseBlocks();
			_exerciseTableDelegate = new ExerciseTableDelegate();
			_exerciseTableDelegate.ExerciseEdited += exerciseTableDelegate_ExerciseEdited;
			_exerciseTableDelegate.ExerciseDeleted += exerciseTableDelegate_ExerciseDeleted;

			ExerciseList.Source = new ExerciseListTableSource(_exercises);
			ExerciseList.Delegate = _exerciseTableDelegate;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			_exerciseTableDelegate.ExerciseEdited -= exerciseTableDelegate_ExerciseEdited;
			_exerciseTableDelegate.ExerciseDeleted -= exerciseTableDelegate_ExerciseDeleted;
		}

		void exerciseTableDelegate_ExerciseEdited(object sender, int exerciseIndex)
		{
			SelectedExerciseId = exerciseIndex;
			PerformSegue(ManageExerciseSegueId, this);
		}

		void exerciseTableDelegate_ExerciseDeleted(object sender, int exerciseIndex)
		{
			Data.DeleteExerciseBlock(_exercises[exerciseIndex].Id);
			_exercises = Data.GetExerciseBlocks();

			InvokeOnMainThread(() =>
			{
				ExerciseList.Source = new ExerciseListTableSource(_exercises);
				ExerciseList.ReloadData();
			});
		}

		public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			if (segue.Identifier.Equals(ManageExerciseSegueId))
			{
				var viewController = (ManageExerciseViewController)segue.DestinationViewController;
				viewController.SelectedExerciseId = SelectedExerciseId;			
			}
		}
	}
}