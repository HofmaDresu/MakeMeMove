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
            DependencyService.Get<IPermissionRequester>().RequestPermissions();
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
            var exerciseId = GetSelectedExerciseId(sender);

            Navigation.PushAsync(new ManageExercise(exerciseId), true);
        }

        private void DeleteExercise(object sender, EventArgs eventArgs)
        {
            var exerciseId = GetSelectedExerciseId(sender);
            var exercise = ViewModel.SelectedExercises.SingleOrDefault(e => e.Id == exerciseId);

            if (exercise == null) return;

            ViewModel.SelectedExercises.Remove(exercise);
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);

            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);

            ViewModel.NotifyExercisesChanged();
        }

        private Guid GetSelectedExerciseId(object sender)
        {
            //TODO: There's got to be a better way to do this
            var parentControl = (sender as Button).ParentView;
            var idControl = parentControl.FindByName<Label>("ExerciseId");
            var id = idControl.Text;
            return Guid.Parse(id);
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
