using System;
using System.Linq;
using Humanizer;
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

            if (exerciseId != null)
            {
                _selectedExercise = _fullSchedule.Exercises.Single(e => e.Id == exerciseId);
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
                _selectedExercise = new ExerciseBlock();
                _isNewExercise = true;
                RepititionEntry.Text = "10";
            }
        }

        private void SaveData(object sender, EventArgs eventArgs)
        {
            int currentIndex;
            if (!_isNewExercise)
            {
                currentIndex = _fullSchedule.Exercises.IndexOf(_selectedExercise);
                _fullSchedule.Exercises.Remove(_fullSchedule.Exercises.Single(e => e.Id == _selectedExercise.Id));
            }
            else
            {
                currentIndex = _fullSchedule.Exercises.Count;
            }

            _selectedExercise.Name = ExerciseTypePicker.SelectedIndex == (int)PreBuiltExersises.Custom
                ? CustomExerciseEntry.Text
                : ((PreBuiltExersises) ExerciseTypePicker.SelectedIndex).Humanize();
            _selectedExercise.Quantity = int.Parse(RepititionEntry.Text);
            _selectedExercise.Id = _selectedExercise.Id ?? new Guid();


            _fullSchedule.Exercises.Insert(currentIndex, _selectedExercise);


            _schedulePersistence.SaveExerciseSchedule(_fullSchedule);
            _notificationServiceManager.RestartNotificationServiceIfNeeded(_fullSchedule);


            Navigation.PopAsync(true);
        }

        private void CancelChanges(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
        }
    }
}
