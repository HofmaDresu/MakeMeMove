using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using MakeMeMove.Model;
using MakeMeMove.iOS.Helpers;
using SWRevealViewControllerBinding;

namespace MakeMeMove.iOS
{
    public partial class MyExercisesController : BaseViewController
    {
		private ExerciseTableDelegate _exerciseTableDelegate;
		private List<ExerciseBlock> _exercises;
		private const string ManageExerciseSegueId = "ManageExerciseSegue";
		public int? SelectedExerciseId;
		private ExerciseListTableSource _source;

        public MyExercisesController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            if (this.RevealViewController() == null) return;

            MenuButton.Clicked += (sender, e) => this.RevealViewController().RevealToggleAnimated(true);
            MenuButton.TintColor = UIColor.White;

            ExerciseList.BackgroundColor = FudistColors.MainBackgroundColor;
			AddExerciseButton.BackgroundColor = FudistColors.PrimaryColor;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			SelectedExerciseId = null;

			_exercises = Data.GetExerciseBlocks();
			_exerciseTableDelegate = new ExerciseTableDelegate();
			_exerciseTableDelegate.ExerciseEdited += exerciseTableDelegate_ExerciseEdited;
			_exerciseTableDelegate.ExerciseDeleted += exerciseTableDelegate_ExerciseDeleted;

			_source = new ExerciseListTableSource(_exercises);
			_source.EnabledDisabledSwitchSelected += EnableDisableExercise;
			ExerciseList.Source = _source;
			ExerciseList.Delegate = _exerciseTableDelegate;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			_exerciseTableDelegate.ExerciseEdited -= exerciseTableDelegate_ExerciseEdited;
			_exerciseTableDelegate.ExerciseDeleted -= exerciseTableDelegate_ExerciseDeleted;
			_source.EnabledDisabledSwitchSelected -= EnableDisableExercise;
		}

		void exerciseTableDelegate_ExerciseEdited(object sender, int exerciseIndex)
		{
			SelectedExerciseId = _exercises[exerciseIndex].Id;
			PerformSegue(ManageExerciseSegueId, this);
		}

		void exerciseTableDelegate_ExerciseDeleted(object sender, int exerciseIndex)
		{
			Data.DeleteExerciseBlock(_exercises[exerciseIndex].Id);
			_exercises = Data.GetExerciseBlocks();

			InvokeOnMainThread(() =>
			{
				_source.EnabledDisabledSwitchSelected -= EnableDisableExercise;
				_source = new ExerciseListTableSource(_exercises);
				_source.EnabledDisabledSwitchSelected += EnableDisableExercise;
				ExerciseList.Source = _source;
				ExerciseList.ReloadData();
				ExerciseList.Delegate = _exerciseTableDelegate;
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

		private void EnableDisableExercise(object sender, ExerciseBlock exercise)
		{
			exercise.Enabled = !exercise.Enabled;
			Data.UpdateExerciseBlock(exercise);
			ServiceManager.RestartNotificationServiceIfNeeded();
		}
	}
}