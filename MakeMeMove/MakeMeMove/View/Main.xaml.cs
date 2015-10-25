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
            EnableDisableButtons(true);
            EnableDisableServiceButtons();
            base.OnAppearing();
            LoadExerciseSchedule();
            DependencyService.Get<IPermissionRequester>().RequestPermissions();
            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);
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
            EnableDisableServiceButtons();
        }

        private void OnStop(object sender, EventArgs e)
        {
            _notificationServiceManager.StopNotificationService(ViewModel.Schedule);
            EnableDisableServiceButtons();
        }

        private void EditExercise(object sender, EventArgs eventArgs)
        {
            EnableDisableButtons(false);
            (sender as Button).IsEnabled = false;
            var exerciseId = GetSelectedExerciseId(sender);

            Navigation.PushAsync(new ManageExercise(exerciseId), true);
        }

        private void DeleteExercise(object sender, EventArgs eventArgs)
        {
            EnableDisableButtons(false);
            (sender as Button).IsEnabled = false;
            var exerciseId = GetSelectedExerciseId(sender);
            var exercise = ViewModel.SelectedExercises.FirstOrDefault(e => e.Id == exerciseId);

            if (exercise == null) return;

            ViewModel.SelectedExercises.Remove(exercise);
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);

            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);

            ViewModel.NotifyExercisesChanged();
            EnableDisableButtons(true);
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
            EnableDisableButtons(false);
            Navigation.PushAsync(new ManageExercise(), true);
        }

        private void EditSchedule(object sender, EventArgs e)
        {
            EnableDisableButtons(false);
            Navigation.PushAsync(new EditSchedule(), true);
        }

        private void EnableDisableButtons(bool enabled)
        {
            AddButton.IsEnabled = enabled;
            EditScheduleButton.IsEnabled = enabled;
            StartServiceButton.IsEnabled = enabled;
            StopServiceButton.IsEnabled = enabled;
        }

        private void EnableDisableServiceButtons()
        {
            if (_notificationServiceManager.NotificationServiceIsRunning())
            {
                StartServiceButton.IsEnabled = false;
                StopServiceButton.IsEnabled = true;
            }
            else
            {
                StartServiceButton.IsEnabled = true;
                StopServiceButton.IsEnabled = false;
            }
        }

        void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}
