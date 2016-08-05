using System;
using System.IO;
using Foundation;
using SQLite;
using UIKit;

namespace MakeMeMove.iOS
{
	public abstract class BaseViewController : UIViewController, IUINavigationBarDelegate
	{
		protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", Constants.DatabaseName)));

		protected BaseViewController(IntPtr handle) : base (handle)
        {
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}
	}
}


