using System;
using Foundation;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseTableDelegate : UITableViewDelegate
	{
		public EventHandler<int> ExerciseEdited;
		public EventHandler<int> ExerciseDeleted;


		public ExerciseTableDelegate()
		{
		}

		public ExerciseTableDelegate(IntPtr handle) : base (handle)
        {
		}

		public ExerciseTableDelegate(NSObjectFlag t) : base (t)
        {
		}

		public override UITableViewRowAction[] EditActionsForRow(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewRowAction editButton = UITableViewRowAction.Create(
				UITableViewRowActionStyle.Normal,
				"Edit", 
				(arg1, arg2) => ExerciseEdited?.Invoke(this, indexPath.Row));
			UITableViewRowAction deleteButton = UITableViewRowAction.Create(
				UITableViewRowActionStyle.Destructive,
				"Delete",
				(arg1, arg2) => ExerciseDeleted?.Invoke(this, indexPath.Row));
			return new UITableViewRowAction[] { deleteButton, editButton };
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 103;
		}
	}
}

