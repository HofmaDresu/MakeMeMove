using Foundation;
using System;
using UIKit;

namespace MakeMeMove.iOS
{
    public partial class MyScheduleController : BaseViewController
    {
        public MyScheduleController (IntPtr handle) : base (handle)
        {
        }

		protected override UINavigationBar GetNavBar() => NavBar;
    }
}