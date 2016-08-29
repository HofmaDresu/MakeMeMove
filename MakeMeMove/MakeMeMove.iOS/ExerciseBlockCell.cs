using Foundation;
using System;
using UIKit;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.Model;

namespace MakeMeMove.iOS
{
    public partial class ExerciseBlockCell : UITableViewCell
    {
		public EventHandler<ExerciseBlock> SelectedEnabledSwitch;
		private ExerciseBlock _block;

        public ExerciseBlockCell (IntPtr handle) : base (handle)
        {
			BackgroundColor = FudistColors.MainBackgroundColor;
        }

		public void UpdateCell(ExerciseBlock block)
		{
			_block = block;
			ExerciseIsEnabled.TintColor = FudistColors.InteractableTextColor;
			ExerciseNameLabel.Text = $"{block.Quantity} {block.CombinedName}";
			ExerciseNameLabel.TextColor = FudistColors.PrimaryColor;
			ExerciseIsEnabled.SelectedSegment = block.Enabled ? 1 : 0;
			ExerciseIsEnabled.ValueChanged -= ExerciseSwitch_ValueChanged;
			ExerciseIsEnabled.ValueChanged += ExerciseSwitch_ValueChanged;
		}

		void ExerciseSwitch_ValueChanged(object sender, EventArgs e)
		{
			SelectedEnabledSwitch?.Invoke(sender, _block);
		}
    }
}