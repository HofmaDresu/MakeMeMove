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
        }

		public void UpdateCell(string exerciseName, int exerciseQuantity, bool isEnabled)
		{
			ExerciseNameLabel.Text = $"{exerciseQuantity} {exerciseName}";
			ExerciseNameLabel.TextColor = FudistColors.PrimaryColor;
			ExerciseIsEnabled.On = isEnabled;
		}
    }
}