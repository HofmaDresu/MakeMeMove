using System;
using System.Linq;
using MakeMeMove.DeviceSpecificInterfaces;
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
        private bool _buttonsAreEnabled = false;

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
            EnableDisableServiceButtons();
            base.OnAppearing();
            LoadExerciseSchedule();
            DependencyService.Get<IPermissionRequester>().RequestPermissions();
            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);
            EnableButtons();
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

        private async void EditExercise(object sender, EventArgs eventArgs)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            var exerciseId = GetSelectedExerciseId(sender);
            var exercise = ViewModel.SelectedExercises.FirstOrDefault(e => e.Id == exerciseId);

            if (exercise == null) return;

            await Navigation.PushAsync(new ManageExercise(exerciseId), true);
            EnableButtons();
        }

        private void DeleteExercise(object sender, EventArgs eventArgs)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            var exerciseId = GetSelectedExerciseId(sender);
            var exercise = ViewModel.SelectedExercises.FirstOrDefault(e => e.Id == exerciseId);

            if (exercise == null) return;

            ViewModel.SelectedExercises.Remove(exercise);
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);

            _notificationServiceManager.RestartNotificationServiceIfNeeded(ViewModel.Schedule);

            ViewModel.NotifyExercisesChanged();
            EnableButtons();
        }

        private Guid GetSelectedExerciseId(object sender)
        {
            //TODO: There's got to be a better way to do this
            var parentControl = (sender as Button).ParentView;
            var idControl = parentControl.FindByName<Label>("ExerciseId");
            var id = idControl.Text;
            return Guid.Parse(id);
        }

        private async void AddExercise(object sender, EventArgs e)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            await Navigation.PushAsync(new ManageExercise(), true);
            EnableButtons();
        }

        private async void EditSchedule(object sender, EventArgs e)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            await Navigation.PushAsync(new EditSchedule(), true);
            EnableButtons();
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

        private void DisableButtons()
        {
            _buttonsAreEnabled = false;
        }

        private void EnableButtons()
        {
            _buttonsAreEnabled = true;
        }

        private bool ButtonsAreEnabled()
        {
            return _buttonsAreEnabled;
        }

        void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }
    }
}
