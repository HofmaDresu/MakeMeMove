using System;
using MakeMeMove.iOS.TableClasses;
using MakeMeMove.iOS.ViewControllers.Base;

namespace MakeMeMove.iOS.ViewControllers
{
    public partial class ExerciseHistoryContainerViewController : BaseViewController
    {
        public ExerciseHistoryContainerViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            ExerciseHistoryTable.Source = new ExerciseHistoryTableSource(Data.GetExerciseHistoryForDay(DateTime.Now.Date));
        }
    }
}