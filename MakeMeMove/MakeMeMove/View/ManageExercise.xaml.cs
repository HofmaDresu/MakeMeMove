using System;
using System.Linq;
using Humanizer;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Model;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class ManageExercise : ContentPage
    {
        private readonly ExerciseSchedule _fullSchedule;
        private readonly ExerciseBlock _selectedExercise;
        private readonly bool _isNewExercise;
        private readonly ISchedulePersistence _schedulePersistence;
        private readonly IServiceManager _notificationServiceManager;
        private bool _buttonsAreEnabled;

        public ManageExercise(Guid? exerciseId = null)
        {
            _schedulePersistence = DependencyService.Get<ISchedulePersistence>();
            _notificationServiceManager = DependencyService.Get<IServiceManager>();
            _fullSchedule = !_schedulePersistence.HasExerciseSchedule() ? ExerciseSchedule.CreateDefaultSchedule() : _schedulePersistence.LoadExerciseSchedule(); ;
            InitializeComponent();

            foreach (PreBuiltExersises suit in Enum.GetValues(typeof(PreBuiltExersises)))
            {
                ExerciseTypePicker.Items.Add(suit.Humanize());
            }
            ExerciseTypePicker.SelectedIndexChanged += (sender, args) =>
            {
                CustomExerciseEntry.IsVisible = (sender as Picker).SelectedIndex == (int) PreBuiltExersises.Custom;
            };

            if (exerciseId != null)
            {
                _selectedExercise = _fullSchedule.Exercises.First(e => e.Id == exerciseId);
                _isNewExercise = false;
                ExerciseTypePicker.SelectedIndex = (int)_selectedExercise.Type;

                if (_selectedExercise.Type == PreBuiltExersises.Custom)
                {
                    CustomExerciseEntry.Text = _selectedExercise.Name;
                    CustomExerciseEntry.IsVisible = true;
                }


                RepititionEntry.Text = _selectedExercise.Quantity.ToString();
            }
            else
            {
                ExerciseTypePicker.SelectedIndex = 0;
                _selectedExercise = new ExerciseBlock();
                _isNewExercise = true;
                RepititionEntry.Text = "10";
            }

            _buttonsAreEnabled = true;
        }

        private void SaveData(object sender, EventArgs eventArgs)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            int currentIndex;
            if (!_isNewExercise)
            {
                currentIndex = _fullSchedule.Exercises.IndexOf(_selectedExercise);
                _fullSchedule.Exercises.Remove(_fullSchedule.Exercises.First(e => e.Id == _selectedExercise.Id));
            }
            else
            {
                currentIndex = _fullSchedule.Exercises.Count;
            }

            _selectedExercise.Name = ExerciseTypePicker.SelectedIndex == (int)PreBuiltExersises.Custom
                ? CustomExerciseEntry.Text
                : ((PreBuiltExersises) ExerciseTypePicker.SelectedIndex).Humanize();
            _selectedExercise.Quantity = int.Parse(RepititionEntry.Text);
            _selectedExercise.Id = _selectedExercise.Id ?? Guid.NewGuid();
            _selectedExercise.Type = (PreBuiltExersises) ExerciseTypePicker.SelectedIndex;


            _fullSchedule.Exercises.Insert(currentIndex, _selectedExercise);


            _schedulePersistence.SaveExerciseSchedule(_fullSchedule);
            _notificationServiceManager.RestartNotificationServiceIfNeeded(_fullSchedule);


            Navigation.PopAsync(true);
        }

        private void CancelChanges(object sender, EventArgs e)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            Navigation.PopAsync(true);
        }

        private void DisableButtons()
        {
            _buttonsAreEnabled = false;
        }

        private bool ButtonsAreEnabled()
        {
            return _buttonsAreEnabled;
        }

        private void RepititionEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int newInt;
            if (!int.TryParse(e.NewTextValue, out newInt))
            {
                RepititionEntry.Text = e.OldTextValue;
            }
        }
    }
}
