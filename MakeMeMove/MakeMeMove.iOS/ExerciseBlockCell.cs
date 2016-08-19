using Foundation;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class ExerciseBlockCell : UITableViewCell
    {
        public ExerciseBlockCell (IntPtr handle) : base (handle)
        {
        }

		public void UpdateCell(string exerciseName, int exerciseQuantity, bool isEnabled)
		{
			ExerciseNameLabel.Text = $"{exerciseQuantity} {exerciseName}";
			ExerciseIsEnabled.On = isEnabled;
		}
    }
}