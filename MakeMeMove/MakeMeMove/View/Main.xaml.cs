using System;
using System.Linq;
using MakeMeMove.Model;
using MakeMeMove.ViewModel;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class Main : ContentPage
    {
        private readonly IServiceManager _notificationServiceManager;
        private readonly ISchedulePersistence _schedulePersistence;
        public ExerciseScheduleViewModel ViewModel;

        public Main()
        {
            _notificationServiceManager = DependencyService.Get<IServiceManager>();
            _schedulePersistence = DependencyService.Get<ISchedulePersistence>();
            ViewModel = new ExerciseScheduleViewModel();
            LoadExerciseSchedule();
            BindingContext = ViewModel;

            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadExerciseSchedule();
        }

        private void LoadExerciseSchedule()
        {
            if (!_schedulePersistence.HasExerciseSchedule())
            {
                ViewModel.Schedule = ExerciseSchedule.CreateDefaultSchedule();
                _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);
            }
            else
            {
                ViewModel.Schedule = _schedulePersistence.LoadExerciseSchedule();
                ViewModel.NotifyExercisesChanged();
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            _notificationServiceManager.StartNotificationService(ViewModel.Schedule);
        }

        private void OnStop(object sender, EventArgs e)
        {
            _notificationServiceManager.StopNotificationService(ViewModel.Schedule);
        }

        private void EditExercise(object sender, EventArgs eventArgs)
        {
            var exercise = GetSelectedExercise(sender);

            Navigation.PushAsync(new ManageExercise(exercise.Id), true);
        }

        private void DeleteExercise(object sender, EventArgs e)
        {
            var exercise = GetSelectedExercise(sender);
            ViewModel.SelectedExercises.Remove(exercise);
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);

            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);

            ViewModel.NotifyExercisesChanged();
        }

        private ExerciseBlock GetSelectedExercise(object sender)
        {
            //TODO: There's got to be a better way to do this
            var parentControl = (sender as Button).ParentView;
            var idControl = parentControl.FindByName<Label>("ExerciseId");
            var id = idControl.Text;
            var exercise = ViewModel.SelectedExercises.Single(e => e.Id == Guid.Parse(id));
            return exercise;
        }

        private void AddExercise(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ManageExercise(), true);
        }

        private void EditSchedule(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditSchedule(), true);
        }
    }
}
