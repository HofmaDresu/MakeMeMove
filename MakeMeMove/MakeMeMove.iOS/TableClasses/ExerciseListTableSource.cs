using System;
using System.Collections.Generic;
using Foundation;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS.TableClasses
{
	public class ExerciseListTableSource : UITableViewSource
	{
		private const string CellIdentifier = "ExerciseBlockCell";
		private readonly List<ExerciseBlock> _exercises;
		public EventHandler<ExerciseBlock> EnabledDisabledSwitchSelected;

		public ExerciseListTableSource(List<ExerciseBlock> exercises)
		{
			_exercises = exercises;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = (ExerciseBlockCell)tableView.DequeueReusableCell(CellIdentifier);
			var exercise = _exercises[indexPath.Row];

			cell.UpdateCell(exercise);
			cell.SelectedEnabledSwitch -= Cell_SelectedEnabledSwitch;
			cell.SelectedEnabledSwitch += Cell_SelectedEnabledSwitch;

			return cell;
		}

		void Cell_SelectedEnabledSwitch(object sender, ExerciseBlock e)
		{
			EnabledDisabledSwitchSelected?.Invoke(sender, e);
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return _exercises.Count;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 103;
		}

		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true; 
		}

        //Don't listen to resharper, this is needed
		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			base.CommitEditingStyle(tableView, editingStyle, indexPath);
		}
	}
}

