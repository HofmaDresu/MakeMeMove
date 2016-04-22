using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using MakeMeMove.Droid.Adapters;
using MakeMeMove.Model;
using AlertDialog = Android.App.AlertDialog;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "Exercise History", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize,
        ParentActivity = typeof(MainActivity))]
    public class ExerciseHistoryActivity : BaseActivity
    {
        private bool _showMarkExercisePrompt;
        private bool _checkForIntentData;
        private int _notifiedExerciseId = -1;
        private Dialog _notificationDialog;
        private Dialog _confirmDeleteDialog;
        private ViewPager _pager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ExerciseHistory);
            ResetPromptData();

            if (savedInstanceState != null)
            {
                _checkForIntentData = savedInstanceState.GetBoolean(Constants.CheckForIntentData, true);
            }
            else
            {
                _checkForIntentData = true;
            }

            if (_checkForIntentData)
            {
                _showMarkExercisePrompt = Intent.GetBooleanExtra(Constants.ShowMarkedExercisePrompt, false);
                _notifiedExerciseId = Intent.GetIntExtra(Constants.ExerciseId, -1);
            }

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
                _pager.SetCurrentItem(_pager.Adapter.Count, false);
                var selectedExercise = Data.GetExerciseById(_notifiedExerciseId);
                if (selectedExercise == null) return;

                ShowTimeToMovePrompt(selectedExercise);
            }
        }

        private void ShowTimeToMovePrompt(ExerciseBlock selectedExercise)
        {
            _notificationDialog?.Dismiss();
            var timeToMoveMessage = string.Format(Resources.GetString(Resource.String.TimeToMoveMessage), selectedExercise.Quantity, selectedExercise.CombinedName);
            _notificationDialog = new AlertDialog.Builder(this)
                .SetTitle(Resource.String.TimeToMoveTitle)
                .SetMessage(timeToMoveMessage)
                .SetCancelable(false)
                .SetPositiveButton(Resource.String.CompletedButtonText, (sender, args) =>
                {
                    Data.MarkExerciseCompleted(selectedExercise.CombinedName, selectedExercise.Quantity);
                    UpdateData();
                    ResetPromptData();
                    _checkForIntentData = false;
                })
                .SetNegativeButton(Resource.String.ChangeExerciseButtonText, (sender, args) =>
                {
                    Data.MarkExerciseNotified(selectedExercise.CombinedName, -1 * selectedExercise.Quantity);

                    var nextExercise =
                        Data.GetNextDifferentEnabledExercise(new ExerciseBlock
                        {
                            Name = selectedExercise.CombinedName,
                            Quantity = selectedExercise.Quantity
                        });
                    if (nextExercise == null)
                    {
                        Toast.MakeText(this, Resource.String.NoAvailableExercises, ToastLength.Long).Show();
                        return;
                    }
                    Data.MarkExerciseNotified(nextExercise.CombinedName, nextExercise.Quantity);

                    UpdateData();
                    ShowTimeToMovePrompt(nextExercise);
                })
                .SetNeutralButton(Resource.String.IgnoreButtonText, (sender, args) =>
                {
                    ResetPromptData();
                    _checkForIntentData = false;
                })
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
                    .SetTitle(Resource.String.DeleteExerciseTitle)
                    .SetMessage(Resource.String.DeleteExerciseMessage)
                    .SetPositiveButton(Resource.String.Yes, (sender, args) =>
                    {
                        Data.DeleteAllHistory();
                        UpdateData();
                    })
                    .SetNegativeButton(Resource.String.No, (sender, args) => { })
                    .Show();

                return true;
            }
            else
            {
                return base.OnOptionsItemSelected(selectedItem);
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutBoolean(Constants.CheckForIntentData, _checkForIntentData);
        }
    }
}