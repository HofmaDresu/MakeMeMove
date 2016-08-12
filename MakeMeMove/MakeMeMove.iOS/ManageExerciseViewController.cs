using Foundation;
using System;
using UIKit;
using MakeMeMove.Model;

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

			if (SelectedExerciseId.HasValue)
			{
				_selectedExercise = Data.GetExerciseById(SelectedExerciseId.Value);

				ExerciseType.Text = _selectedExercise.CombinedName;
				NumberOfRepetitions.Text = _selectedExercise.Quantity.ToString();
				CustomExerciseName.Text = _selectedExercise.Name;	
			}
		}
    }
}