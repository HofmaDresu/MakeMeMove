using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.Model;
using MakeMeMove.ViewModel;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class Main : ContentPage
    {
        private readonly IServiceManager _notificationServiceManager;
        public ExerciseScheduleViewModel ViewModel;

        public Main()
        {
            ViewModel = new ExerciseScheduleViewModel();

            InitializeComponent();
            _notificationServiceManager = DependencyService.Get<IServiceManager>();
            ViewModel.Schedule = ExerciseSchedule.CreateDefaultSchedule();
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
            throw new NotImplementedException();
        }

        private void DeleteExercise(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddExercise(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
