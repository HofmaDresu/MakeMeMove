using System;
using System.ServiceModel.Channels;
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
            ViewModel = new ExerciseScheduleViewModel();

            InitializeComponent();
            BindingContext = ViewModel;

            _notificationServiceManager = DependencyService.Get<IServiceManager>();
            _schedulePersistence = DependencyService.Get<ISchedulePersistence>();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!_schedulePersistence.HasExerciseSchedule())
            {
                ViewModel.Schedule = ExerciseSchedule.CreateDefaultSchedule();
                _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);
            }
            else
            {
                ViewModel.Schedule = _schedulePersistence.LoadExerciseSchedule();

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

        private void EditExercise(object sender, EventArgs e)
        {
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);
            throw new NotImplementedException();
        }

        private void DeleteExercise(object sender, EventArgs e)
        {
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);
            throw new NotImplementedException();
        }

        private void AddExercise(object sender, EventArgs e)
        {
            _schedulePersistence.SaveExerciseSchedule(ViewModel.Schedule);
            throw new NotImplementedException();
        }

        private void EditSchedule(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditSchedule(), true);
        }
    }
}
