using System;
using Foundation;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseTableDelegate : UITableViewDelegate
	{
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
				delegate
				{
					Console.WriteLine("Hello World!");
				});
			UITableViewRowAction deleteButton = UITableViewRowAction.Create(
				UITableViewRowActionStyle.Destructive,
				"Delete",
				delegate
				{
					Console.WriteLine("Hello World!");
				});
			return new UITableViewRowAction[] { deleteButton, editButton };
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 103;
		}
	}
}

