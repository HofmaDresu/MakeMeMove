using System;
using System.Linq;
using Humanizer;
using MakeMeMove.iOS.Controls;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.Models;
using MakeMeMove.iOS.ViewControllers.Base;
using MakeMeMove.Model;
using SWRevealViewControllerBinding;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
	public partial class ManageExerciseViewController : BaseTabbedViewController
    {
		public int? SelectedExerciseId;
		private ExerciseBlock _selectedExercise;
		private UIPickerView _exerciseTypePicker;
		private UIPickerView _repetitionTypePicker;
		private FloatingButton _saveButton;
		private FloatingButton _cancelButton;

        public ManageExerciseViewController (IntPtr handle) : base (handle)
        {
            ScreenName = "Manage Exercise";
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var exercisePickerModel = new PickerModel(PickerListHelper.GetExerciseTypeStrings());
			_exerciseTypePicker = MirroredPicker.Create(exercisePickerModel, ExerciseType, doneAction: ShowHideCustomName);

			var repetitionsPickerModel = new PickerModel(Enumerable.Range(1, 100).Select(n => n.ToString()).ToList());
			_repetitionTypePicker = MirroredPicker.Create(repetitionsPickerModel, NumberOfRepetitions, doneAction: ShowHideCustomName);

			if (SelectedExerciseId.HasValue)
			{
				_selectedExercise = Data.GetExerciseById(SelectedExerciseId.Value);

				ExerciseType.Text = _selectedExercise.Type.Humanize();
				_exerciseTypePicker.Select((int)_selectedExercise.Type, 0, false);
				NumberOfRepetitions.Text = _selectedExercise.Quantity.ToString();
				_repetitionTypePicker.Select(_selectedExercise.Quantity - 1, 0 , false);
				CustomExerciseName.Text = _selectedExercise.CombinedName;
			}
			else
			{
				ExerciseType.Text = PickerListHelper.GetExerciseTypeStrings()[0];
				_exerciseTypePicker.Select(0, 0, false);
				NumberOfRepetitions.Text = "10";
				_repetitionTypePicker.Select(9, 0, false);
				CustomExerciseName.Text = string.Empty;
			}
            
			ShowHideCustomName();
			AddButtons();

			var pickerTextFields = View.Subviews.OfType<PickerUITextField>().ToArray();
			Colors.SetTextPrimaryColor(pickerTextFields);
			CustomExerciseName.BackgroundColor = Colors.MainBackgroundColor;
			CustomExerciseName.TextColor = Colors.PrimaryColor;
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
            ShowHideCustomName();
            var exerciseType = (PreBuiltExersises)(int)_exerciseTypePicker.SelectedRowInComponent(0);
			if (exerciseType == PreBuiltExersises.Custom && string.IsNullOrWhiteSpace(CustomExerciseName.Text))
			{
				GeneralAlertDialogs.ShowValidationErrorPopUp(this, "Please enter a name for your exercise.");
				return;
			}

			if (_selectedExercise != null)
			{
				_selectedExercise.Name = exerciseType == PreBuiltExersises.Custom ? CustomExerciseName.Text : string.Empty;
				_selectedExercise.Quantity = (int)_repetitionTypePicker.SelectedRowInComponent(0) + 1;
				_selectedExercise.Type = exerciseType;
				Data.UpdateExerciseBlock(_selectedExercise);
			}
			else
			{
				Data.InsertExerciseBlock(new ExerciseBlock
				{
					Name = exerciseType == PreBuiltExersises.Custom ? CustomExerciseName.Text : string.Empty,
					Quantity = (int)_repetitionTypePicker.SelectedRowInComponent(0) + 1,
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
		    const int expandedCustomTextConstraint = 30;
		    var customStatusChanged =
		        new Func<bool, bool>(
		            isCustom => (CustomTextHeightConstraint.Constant == expandedCustomTextConstraint && !isCustom)
		                          || (CustomTextHeightConstraint.Constant != expandedCustomTextConstraint && isCustom));

			var exerciseIsCustom = _exerciseTypePicker.SelectedRowInComponent(0) == (int)PreBuiltExersises.Custom;

		    if (customStatusChanged(exerciseIsCustom))
		    {
                CustomTextHeightConstraint.Constant = exerciseIsCustom ? expandedCustomTextConstraint : 0;
                CustomExerciseName.Hidden = !exerciseIsCustom;
                if (!exerciseIsCustom)
                {
                    CustomExerciseName.Text = string.Empty;
                }
                else
                {
                    CustomExerciseName.BecomeFirstResponder();
                }
            }
		}

		public override void ViewWillAppear(bool animated)
        {
            if (this.RevealViewController() != null)
            {
                this.RevealViewController().PanGestureRecognizer.Enabled = false;
            }

            base.ViewWillAppear(animated);
			_saveButton.TouchUpInside += SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside += CancelButtonTouchUpInside;
		}

		public override void ViewWillDisappear(bool animated)
        {
            if (this.RevealViewController() != null)
            {
                this.RevealViewController().PanGestureRecognizer.Enabled = true;
            }

            base.ViewWillDisappear(animated);
			_saveButton.TouchUpInside -= SaveButtonTouchUpInside;
			_cancelButton.TouchUpInside -= CancelButtonTouchUpInside;
		}
    }
}