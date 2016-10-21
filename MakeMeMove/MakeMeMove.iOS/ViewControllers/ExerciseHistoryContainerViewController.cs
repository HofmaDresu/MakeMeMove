using System;
using System.IO;
using MakeMeMove.iOS.Helpers;
using MakeMeMove.iOS.TableClasses;
using MakeMeMove.iOS.ViewControllers.Base;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class ExerciseHistoryContainerViewController : UIViewController
    {
        private readonly Data _data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));

        public ExerciseHistoryContainerViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ExerciseHistoryTable.BackgroundColor = FudistColors.MainBackgroundColor;
            
        }

        public void UpdateData(DateTime date)
        {
            ExerciseHistoryTable.Source = new ExerciseHistoryTableSource(_data.GetExerciseHistoryForDay(date.Date));
            ExerciseHistoryTable.ReloadData();
        }
    }
}