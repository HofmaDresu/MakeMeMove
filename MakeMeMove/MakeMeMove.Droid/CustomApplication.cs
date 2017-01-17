using System;
using Android.App;
using SQLite;
using System.IO;
using Android.Runtime;

namespace MakeMeMove.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
[Application(Debuggable=false)]
#endif
    public class CustomApplication : Application
    {
        public CustomApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName))).IncrementRatingCycle();
        }
    }
}