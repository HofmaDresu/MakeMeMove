using System.IO;
using Android.App;
using SQLite;
using Environment = System.Environment;

namespace MakeMeMove.Droid.Activities
{
    public class BaseActivity : Activity
    {
        protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));
    }
}