using System;
using System.Collections.Generic;
using Foundation;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.TableClasses;
using MakeMeMove.iOS.ViewControllers.Base;
using MakeMeMove.Model;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class MyExercisesController : BaseTabbedViewController
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

            MenuButton.TintColor = UIColor.White;

            ExerciseList.BackgroundColor = FudistColors.MainBackgroundColor;
			AddExerciseButton.BackgroundColor = FudistColors.PrimaryColor;
		}

        private void MenuButton_Clicked(object sender, EventArgs e)
        {
            this.RevealViewController().RevealToggleAnimated(true);
        }

        public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			SelectedExerciseId = null;
            MenuButton.Clicked += MenuButton_Clicked;

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
            MenuButton.Clicked -= MenuButton_Clicked;
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
			ServiceManager.RestartNotificationServiceIfNeeded();

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