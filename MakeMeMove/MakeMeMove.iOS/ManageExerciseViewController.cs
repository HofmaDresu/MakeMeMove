using Foundation;
using System;
using UIKit;
using MakeMeMove.Model;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Models;
using Humanizer;
using MakeMeMove.iOS.Helpers;
using System.Linq;

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

			NumberOfRepetitions.KeyboardType = UIKeyboardType.NumberPad;

			var exercisePickerModel = new PickerModel(PickerListHelper.GetExerciseTypeStrings());
			_exerciseTypePicker = MirroredPicker.Create(exercisePickerModel, ExerciseType, doneAction: ShowHideCustomName);

			if (SelectedExerciseId.HasValue)
			{
				_selectedExercise = Data.GetExerciseById(SelectedExerciseId.Value);

				ExerciseType.Text = _selectedExercise.Type.Humanize();
				_exerciseTypePicker.Select((int)_selectedExercise.Type, 0, false);
				NumberOfRepetitions.Text = _selectedExercise.Quantity.ToString();
				CustomExerciseName.Text = _selectedExercise.CombinedName;
			}
			else
			{
				ExerciseType.Text = PickerListHelper.GetExerciseTypeStrings()[0];
				_exerciseTypePicker.Select(0, 0, false);
				NumberOfRepetitions.Text = "10";
				CustomExerciseName.Text = string.Empty;
			}

			ShowHideCustomName();
			AddButtons();

			var pickerTextFields = View.Subviews.OfType<PickerUITextField>().ToArray();
			FudistColors.SetTextPrimaryColor(pickerTextFields);
			NumberOfRepetitions.BackgroundColor = FudistColors.MainBackgroundColor;
			NumberOfRepetitions.TextColor = FudistColors.PrimaryColor;
		}

		private void AddButtons()
		{
			_saveButton = new FloatingButton("Save");
			View.Add(_saveButton);

			_cancelButton = new FloatingButton("Cancel");
			View.Add(_cancelButton);


			_saveButton.TopAnchor.ConstraintEqualTo(NumberOfRepetitions.BottomAnchor, 20).Active = true;
			_saveButton.LeftAnchor.ConstraintEqualTo(NumberOfRepetitions.LeftAnchor).Active = true;

			_cancelButton.TopAnchor.ConstraintEqualTo(NumberOfRepetitions.BottomAnchor, 20).Active = true;
			_cancelButton.LeftAnchor.ConstraintEqualTo(_saveButton.RightAnchor, 20).Active = true;
		}

		private void SaveButtonTouchUpInside(object sender, EventArgs e)
		{
			var exerciseType = (PreBuiltExersises)(int)_exerciseTypePicker.SelectedRowInComponent(0);
			if (exerciseType == PreBuiltExersises.Custom && string.IsNullOrWhiteSpace(CustomExerciseName.Text))
			{
				GeneralAlertDialogs.ShowValidationErrorPopUp(this, "Please enter a name for your exercise.");
				return;
			}

			if (string.IsNullOrWhiteSpace(NumberOfRepetitions.Text))
			{
				GeneralAlertDialogs.ShowValidationErrorPopUp(this, "Please enter how many repetitions you want.");
				return;
			}
			int repetitions;
			if (!int.TryParse(NumberOfRepetitions.Text, out repetitions))
			{
				GeneralAlertDialogs.ShowValidationErrorPopUp(this, "Please enter a whole number of repetitions.");
				return;
			}

			if (_selectedExercise != null)
			{
				_selectedExercise.Name = exerciseType == PreBuiltExersises.Custom ? CustomExerciseName.Text : string.Empty;
				_selectedExercise.Quantity = repetitions;
				_selectedExercise.Type = exerciseType;
				Data.UpdateExerciseBlock(_selectedExercise);
			}
			else
			{
				Data.InsertExerciseBlock(new ExerciseBlock
				{
					Name = exerciseType == PreBuiltExersises.Custom ? CustomExerciseName.Text : string.Empty,
					Quantity = repetitions,
					Type = exerciseType,
					Enabled = true
				});

			}

			ServiceManager.RestartNotificationServiceIfNeeded();
			NavigationController.PopViewController(true);
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
			if (!exerciseIsCustom)
			{
				CustomExerciseName.Text = string.Empty;
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			_saveButton.TouchUpInside += SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside += CancelButtonTouchUpInside;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			_saveButton.TouchUpInside -= SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside -= CancelButtonTouchUpInside;
		}
    }
}