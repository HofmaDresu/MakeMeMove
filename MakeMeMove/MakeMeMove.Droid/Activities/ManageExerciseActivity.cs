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
using Humanizer;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;

namespace MakeMeMove.Droid.Activities
{
    [Activity(Label = "ManageExerciseActivity", Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class ManageExerciseActivity : Activity
    {
        private Spinner _exerciseTypeSpinner;
        private EditText _customExerciseNameText;
        private EditText _repetitionText;
        private Button _saveButton;
        private Button _cancelButton;

        private readonly ISchedulePersistence _schedulePersistence = new SchedulePersistence();
        private ExerciseSchedule _exerciseSchedule;
        private readonly UserNotification _userNotification = new UserNotification();
        private Guid? _currentExerciseId = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _exerciseSchedule = _schedulePersistence.LoadExerciseSchedule();

            SetContentView(Resource.Layout.ManageExercise);

            _exerciseTypeSpinner = FindViewById<Spinner>(Resource.Id.ExerciseTypeSpinner);
            _customExerciseNameText = FindViewById<EditText>(Resource.Id.CustomExerciseNameBox);
            _repetitionText = FindViewById<EditText>(Resource.Id.RepetitionsText);
            _saveButton = FindViewById<Button>(Resource.Id.SaveButton);
            _cancelButton = FindViewById<Button>(Resource.Id.CancelButton);

            InitializePickers();

            var editExerciseId = Intent.GetStringExtra(Constants.ExerciseId);
            if (!string.IsNullOrWhiteSpace(editExerciseId))
            {
                _currentExerciseId = Guid.Parse(editExerciseId);
                SetCurrentExerciseData();
                Title = "Edit Exercise";
            }
            else
            {
                Title = "Add Exercise";
            }

            _cancelButton.Click += (s, e) => Finish();
            _saveButton.Click += (s, e) => SaveData();
        }

        private void InitializePickers()
        {
            var exerciseList = (from PreBuiltExersises suit in Enum.GetValues(typeof(PreBuiltExersises)) select suit.Humanize()).ToList();
            _exerciseTypeSpinner.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, exerciseList);

            _exerciseTypeSpinner.ItemSelected += (sender, args) => ShowHideCustomText();
        }

        private void SetCurrentExerciseData()
        {
            var currentExercise = _exerciseSchedule.Exercises.Single(e => e.Id == _currentExerciseId);

            _exerciseTypeSpinner.SetSelection((int) currentExercise.Type);
            _repetitionText.Text = currentExercise.Quantity.ToString();
            _customExerciseNameText.Text = currentExercise.Type == PreBuiltExersises.Custom ? currentExercise.Name : string.Empty;
        }

        private void ShowHideCustomText()
        {
            var selectedExerciseType = (PreBuiltExersises)_exerciseTypeSpinner.SelectedItemPosition;
            _customExerciseNameText.Visibility = selectedExerciseType == PreBuiltExersises.Custom ? ViewStates.Visible : ViewStates.Gone;
        }

        private void SaveData()
        {
            var exerciseType = (PreBuiltExersises)_exerciseTypeSpinner.SelectedItemPosition;
            if (exerciseType == PreBuiltExersises.Custom && string.IsNullOrWhiteSpace(_customExerciseNameText.Text))
            {
                _userNotification.ShowValidationErrorPopUp(this, "Please enter a name for your exercise.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_repetitionText.Text))
            {
                _userNotification.ShowValidationErrorPopUp(this, "Please enter how many repetitions you want.");
                return;
            }
            int repetitions;
            if (!int.TryParse(_repetitionText.Text, out repetitions))
            {
                _userNotification.ShowValidationErrorPopUp(this, "Please enter a whole number of repetitions.");
                return;
            }

            if (_currentExerciseId.HasValue)
            {
                var exercise = _exerciseSchedule.Exercises.Single(e => e.Id == _currentExerciseId);

                exercise.Name = exerciseType == PreBuiltExersises.Custom ? _customExerciseNameText.Text : string.Empty;
                exercise.Quantity = repetitions;
                exercise.Type = exerciseType;
            }
            else
            {
                _exerciseSchedule.Exercises.Add(new ExerciseBlock
                {
                    Id = Guid.NewGuid(),
                    Name = exerciseType == PreBuiltExersises.Custom ? _customExerciseNameText.Text : string.Empty,
                    Quantity = repetitions,
                    Type = exerciseType
                });

            }

            _schedulePersistence.SaveExerciseSchedule(_exerciseSchedule);

            Finish();
        }


    }
}