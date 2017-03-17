using Android.App;
using Android.Content.PM;
using Android.OS;
using System.Linq;
using Android.Widget;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Exercise Totals", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize,
        ParentActivity = typeof(MainActivity))]
    public class ExercisTotalsActivity : BaseActivity
    {
        private ListView _stats;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExerciseTotals);
            _stats = FindViewById<ListView>(Resource.Id.Stats);
            UpdateData();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateData();
        }

        private void UpdateData()
        {
            var totals = Data.GetExerciseTotals();

            _stats.Adapter = new ArrayAdapter(this, Resource.Layout.ExerciseHistoryListItem,
                totals
                    .Select(s => $"{s.ExerciseName}: {s.QuantityCompleted}")
                    .ToList());
        }
    }
}