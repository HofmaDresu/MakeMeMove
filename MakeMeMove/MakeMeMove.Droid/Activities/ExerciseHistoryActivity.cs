using System.IO;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Model;
using SQLite;
using AlertDialog = Android.App.AlertDialog;
using Environment = System.Environment;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Exercise History", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize,
        ParentActivity = typeof(MainActivity))]
    public class ExerciseHistoryActivity : BaseActivity
    {
        private bool _showMarkExercisePrompt;
        private int _notifiedExerciseId = -1;
        private Dialog _notificationDialog;
        private Dialog _confirmDeleteDialog;
        private ViewPager _pager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExerciseHistory);
            ResetPromptData();

            _showMarkExercisePrompt = Intent.GetBooleanExtra(Constants.ShowMarkedExercisePrompt, false);
            _notifiedExerciseId = Intent.GetIntExtra(Constants.ExerciseId, -1);

            var adapter = new ExerciseHistoryFragmentAdapter(FragmentManager);

            _pager = FindViewById<ViewPager>(Resource.Id.historyPager);
            _pager.Adapter = adapter;
            _pager.SetCurrentItem(adapter.Count, false);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateData();

            if (_showMarkExercisePrompt)
            {
                var selectedExercise = Data.GetExerciseById(_notifiedExerciseId);
                if (selectedExercise == null) return;

                ShowTimeToMovePrompt(selectedExercise);
            }
        }

        private void ShowTimeToMovePrompt(ExerciseBlock selectedExercise)
        {
            _notificationDialog?.Dismiss();
            _notificationDialog = new AlertDialog.Builder(this)
                .SetTitle("It's time to move")
                .SetMessage($"It's time to do {selectedExercise.Quantity} {selectedExercise.CombinedName}")
                .SetCancelable(false)
                .SetPositiveButton("Completed", (sender, args) =>
                {
                    Data.MarkExerciseCompleted(selectedExercise.CombinedName, selectedExercise.Quantity);
                    UpdateData();
                    ResetPromptData();
                })
                .SetNegativeButton("Next", (sender, args) =>
                {
                    Data.MarkExerciseNotified(selectedExercise.CombinedName, -1 * selectedExercise.Quantity);

                    var nextExercise = Data.GetNextEnabledExercise();
                    Data.MarkExerciseNotified(nextExercise.CombinedName, nextExercise.Quantity);

                    UpdateData();
                    ShowTimeToMovePrompt(nextExercise);
                })
                .SetNeutralButton("Ignore", (sender, args) => ResetPromptData())
                .Show();
        }

        private void UpdateData()
        {
            var currentPosition = _pager.CurrentItem;
            var adapter = new ExerciseHistoryFragmentAdapter(FragmentManager);
            
            _pager.Adapter = adapter;
            _pager.SetCurrentItem(currentPosition, false);
        }

        private void ResetPromptData()
        {
            _showMarkExercisePrompt = false;
            _notifiedExerciseId = -1;
        }

        protected override void OnPause()
        {
            base.OnPause();
            _notificationDialog?.Dismiss();
            _confirmDeleteDialog?.Dismiss();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.HistorySettings, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem selectedItem)
        {
            if (selectedItem.ItemId == Resource.Id.DeleteHistory)
            {
                _confirmDeleteDialog?.Dismiss();
                _confirmDeleteDialog = new AlertDialog.Builder(this)
                    .SetTitle("Delete Exercise History")
                    .SetMessage("This will permanently delete your exercise history. Do you wish to continue?")
                    .SetPositiveButton("Yes", (sender, args) =>
                    {
                        Data.DeleteAllHistory();
                        UpdateData();
                    })
                    .SetNegativeButton("No", (sender, args) => { })
                    .Show();

                return true;
            }
            else
            {
                return base.OnOptionsItemSelected(selectedItem);
            }
        }
    }
}