using System;
using System.Collections.Generic;
using Foundation;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS.TableClasses
{
	public class ExerciseHistoryTableSource : UITableViewSource
	{
		private const string CellIdentifier = "ExerciseHistoryCell";
		private readonly List<ExerciseHistory> _historyItems;

		public ExerciseHistoryTableSource(List<ExerciseHistory> historyItems)
		{
            _historyItems = historyItems;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ExerciseHistoryCell)tableView.DequeueReusableCell(CellIdentifier);
			var exercise = _historyItems[indexPath.Row];

			cell.UpdateCell(exercise);

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _historyItems.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 44;
		}
	}
}

