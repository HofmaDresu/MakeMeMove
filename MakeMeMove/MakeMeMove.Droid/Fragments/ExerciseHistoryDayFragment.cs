using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MakeMeMove.Model;
using Newtonsoft.Json;

namespace MakeMeMove.Droid.Fragments
{
    public class ExerciseHistoryDayFragment : Fragment
    {
        private List<ExerciseHistory> _exerciseHistoryStats;

        public ExerciseHistoryDayFragment() { }

        public ExerciseHistoryDayFragment(List<ExerciseHistory> stats)
        {
            _exerciseHistoryStats = stats;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (savedInstanceState != null)
            {
                _exerciseHistoryStats = JsonConvert.DeserializeObject<List<ExerciseHistory>>(savedInstanceState.GetString(Constants.ExerciseHistoryItem));
            }

            var view = inflater.Inflate(Resource.Layout.ExerciseHistoryFragment, container, false);
            var stats = view.FindViewById<ListView>(Resource.Id.Stats);
            
            stats.Adapter = new ArrayAdapter(Activity, Resource.Layout.ExerciseHistoryListItem,
                _exerciseHistoryStats
                    .Select(s => $"{s.ExerciseName}: {s.QuantityCompleted} / {s.QuantityNotified}")
                    .ToList());

            return view;
        }
        
        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(Constants.ExerciseHistoryItem, JsonConvert.SerializeObject(_exerciseHistoryStats));
        }
    }
}