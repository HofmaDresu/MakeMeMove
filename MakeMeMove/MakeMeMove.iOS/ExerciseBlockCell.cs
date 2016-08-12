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

		public void UpdateCell(string exerciseName, bool isEnabled)
		{
			ExerciseNameLabel.Text = exerciseName;
			ExerciseIsEnabled.On = isEnabled;
		}
    }
}