using System.IO;
using Android.App;
using Android.Support.V7.App;
using SQLite;
using Environment = System.Environment;

namespace MakeMeMove.Droid.Activities
{
    public class BaseActivity : AppCompatActivity
    {
        protected readonly Data Data = Data.GetInstance(new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), Constants.DatabaseName)));
    }
}