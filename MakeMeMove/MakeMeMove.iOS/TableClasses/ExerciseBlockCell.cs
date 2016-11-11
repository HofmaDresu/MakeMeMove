using System;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS.TableClasses
{
    public partial class ExerciseBlockCell : UITableViewCell
    {
		public EventHandler<ExerciseBlock> SelectedEnabledSwitch;
		private ExerciseBlock _block;

        public ExerciseBlockCell (IntPtr handle) : base (handle)
        {
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
		    BackgroundColor = block.Enabled ? FudistColors.MainBackgroundColor : FudistColors.DisabledBackgroundColor;
		}

		void ExerciseSwitch_ValueChanged(object sender, EventArgs e)
		{
            BackgroundColor = ExerciseIsEnabled.SelectedSegment == 1 ? FudistColors.MainBackgroundColor : FudistColors.DisabledBackgroundColor;
            SelectedEnabledSwitch?.Invoke(sender, _block);
		}
    }
}