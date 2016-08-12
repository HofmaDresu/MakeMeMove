using Foundation;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class MyExercisesController : BaseViewController
    {
        public MyExercisesController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ExerciseList.Source = new ExerciseListTableSource(Data.GetExerciseBlocks());
		}
    }
}