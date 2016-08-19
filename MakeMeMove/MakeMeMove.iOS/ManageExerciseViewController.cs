using Foundation;
using System;
using UIKit;
using MakeMeMove.Model;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Models;

namespace MakeMeMove.iOS
{
	public partial class ManageExerciseViewController : BaseViewController
    {
		public int? SelectedExerciseId;
		private ExerciseBlock _selectedExercise;
		private UIPickerView _exerciseTypePicker;
		private FloatingButton _saveButton;
		private FloatingButton _cancelButton;

        public ManageExerciseViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			var exercisePickerModel = new PickerModel(PickerListHelper.GetExerciseTypeStrings());
			_exerciseTypePicker = MirroredPicker.Create(exercisePickerModel, ExerciseType, doneAction: ShowHideCustomName);

			if (SelectedExerciseId.HasValue)
			{
				_selectedExercise = Data.GetExerciseById(SelectedExerciseId.Value);

				ExerciseType.Text = _selectedExercise.CombinedName;
				_exerciseTypePicker.Select((int)_selectedExercise.Type, 0, false);
				NumberOfRepetitions.Text = _selectedExercise.Quantity.ToString();
				CustomExerciseName.Text = _selectedExercise.Name;
			}
			else
			{
				ExerciseType.Text = PickerListHelper.GetExerciseTypeStrings()[0];
				_exerciseTypePicker.Select(0, 0, false);
				NumberOfRepetitions.Text = "10";;
				CustomExerciseName.Text = string.Empty;
			}

			ShowHideCustomName();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			var buttonYPosition = NumberOfRepetitions.Frame.Height + NumberOfRepetitions.Frame.Y + 20;

			_saveButton = new FloatingButton("Save", ExerciseType.Frame.X, buttonYPosition, View);
			_saveButton.TouchUpInside += SaveButtonTouchUpInside;
			View.Add(_saveButton);

			_cancelButton = new FloatingButton("Cancel", (ExerciseType.Frame.X * 2) + _saveButton.Frame.Width, buttonYPosition, View);
			_cancelButton.TouchUpInside += CancelButtonTouchUpInside;
			View.Add(_cancelButton);
		}

		private void SaveButtonTouchUpInside(object sender, EventArgs e)
		{

		}

		private void CancelButtonTouchUpInside(object sender, EventArgs e)
		{
			NavigationController.PopViewController(true);
		}

		private void ShowHideCustomName()
		{
			var exerciseIsCustom = _exerciseTypePicker.SelectedRowInComponent(0) == (int)PreBuiltExersises.Custom;
			CustomTextHeightConstraint.Constant = exerciseIsCustom ? 30 : 0;
			CustomExerciseName.Hidden = !exerciseIsCustom;
			CustomExerciseName.Text = string.Empty;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			_saveButton.TouchUpInside -= SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside -= CancelButtonTouchUpInside;
		}
    }
}