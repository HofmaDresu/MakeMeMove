using System;
using System.Collections.Generic;
using Foundation;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS
{
	public class ExerciseListTableSource : UITableViewSource
	{
		private const string CellIdentifier = "ExerciseBlockCell";
		private readonly List<ExerciseBlock> _exercises;

		public ExerciseListTableSource(List<ExerciseBlock> exercises)
		{
			_exercises = exercises;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ExerciseBlockCell)tableView.DequeueReusableCell(CellIdentifier);
			var exercise = _exercises[indexPath.Row];

			cell.UpdateCell(exercise.CombinedName, exercise.Enabled);

			return cell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _exercises.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 103;
		}
	}
}

