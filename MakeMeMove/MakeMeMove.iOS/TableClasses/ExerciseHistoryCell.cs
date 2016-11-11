using System;
using System.Linq;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.Model;
using UIKit;

namespace MakeMeMove.iOS.TableClasses
{
    public partial class ExerciseHistoryCell : UITableViewCell
    {
        public ExerciseHistoryCell (IntPtr handle) : base (handle)
        {
            BackgroundColor = FudistColors.MainBackgroundColor;
        }

        public void UpdateCell(ExerciseHistory history)
        {
            ExerciseName.Text = history.ExerciseName + ":";
            ExerciseName.TextColor = FudistColors.PrimaryColor;
            ExerciseCountLabel.Text = $"{history.QuantityCompleted} Completed";
            ExerciseCountLabel.TextColor = FudistColors.PrimaryColor;
        }
    }
}