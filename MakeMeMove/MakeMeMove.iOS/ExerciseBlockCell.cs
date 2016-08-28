using Foundation;
using System;
using UIKit;
using MakeMeMove.iOS.Helpers;

namespace MakeMeMove.iOS
{
    public partial class ExerciseBlockCell : UITableViewCell
    {
        public ExerciseBlockCell (IntPtr handle) : base (handle)
        {
			BackgroundColor = FudistColors.MainBackgroundColor;
			//TODO: Add functionality to ExerciseIsEnabled
        }

		public void UpdateCell(string exerciseName, int exerciseQuantity, bool isEnabled)
		{
			ExerciseIsEnabled.TintColor = FudistColors.InteractableTextColor;
			ExerciseNameLabel.Text = $"{exerciseQuantity} {exerciseName}";
			ExerciseNameLabel.TextColor = FudistColors.PrimaryColor;
			ExerciseIsEnabled.SelectedSegment = isEnabled ? 1 : 0;
		}
    }
}