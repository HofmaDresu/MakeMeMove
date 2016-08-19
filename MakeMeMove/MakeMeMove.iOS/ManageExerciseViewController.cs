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

        public ManageExerciseViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			var exercisePickerModel = new PickerModel(PickerListHelper.GetExerciseTypeStrings());
			var exerciseTypePicker = MirroredPicker.Create(exercisePickerModel, ExerciseType);

			if (SelectedExerciseId.HasValue)
			{
				_selectedExercise = Data.GetExerciseById(SelectedExerciseId.Value);

				ExerciseType.Text = _selectedExercise.CombinedName;
				exerciseTypePicker.Select((int)_selectedExercise.Type, 0, false);
				NumberOfRepetitions.Text = _selectedExercise.Quantity.ToString();
				CustomExerciseName.Text = _selectedExercise.Name;	
			}
			else
			{
				ExerciseType.Text = PickerListHelper.GetExerciseTypeStrings()[0];
				exerciseTypePicker.Select(0, 0, false);
				NumberOfRepetitions.Text = "10";;
				CustomExerciseName.Text = string.Empty;
			}
		}
    }
}