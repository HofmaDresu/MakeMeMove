using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Exercise History", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ExerciseHistoryActivity : BaseActivity
    {
        private TextView _date;
        private ListView _stats;
        private bool _showMarkExercisePrompt;
        private int _notifiedExerciseId = -1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExerciseHistory);
            ResetPromptData();

            _showMarkExercisePrompt = Intent.GetBooleanExtra(Constants.ShowMarkedExercisePrompt, false);
            _notifiedExerciseId = Intent.GetIntExtra(Constants.ExerciseId, -1);


            _date = FindViewById<TextView>(Resource.Id.Date);
            _stats = FindViewById<ListView>(Resource.Id.Stats);
        }

        protected override void OnResume()
        {
            base.OnResume();
            var todaysStats = Data.GetExerciseHistoryForDay(DateTime.Today);

            _date.Text = DateTime.Today.ToShortDateString();

            _stats.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, todaysStats.Where(s => s.QuantityNotified > 0).Select(s => $"{s.ExerciseName}: {s.QuantityCompleted} / {s.QuantityNotified}").ToList());

            if (_showMarkExercisePrompt)
            {
                var selectedExercise = Data.GetExerciseById(_notifiedExerciseId);
                if (selectedExercise == null) return;

                new AlertDialog.Builder(this)
                    .SetTitle("It's time to move")
                    .SetMessage($"It's time to do {selectedExercise.Quantity} {selectedExercise.CombinedName}")
                    .SetCancelable(false)
                    .SetPositiveButton("Completed", (sender, args) =>
                    {
                        Data.MarkExerciseCompleted(selectedExercise.CombinedName, selectedExercise.Quantity);
                        ResetPromptData();
                    })
                    .SetNegativeButton("Next", (sender, args) =>
                    {
                        ResetPromptData();
                    })
                    .SetNeutralButton("Ignore", (sender, args) => ResetPromptData())
                    .Show();
            }
        }

        private void ResetPromptData()
        {
            _showMarkExercisePrompt = false;
            _notifiedExerciseId = -1;
        }
    }
}