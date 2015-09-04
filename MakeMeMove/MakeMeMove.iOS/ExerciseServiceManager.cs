using System;
using System.Collections.Generic;
using System.Text;
using MakeMeMove.iOS;
using MakeMeMove.Model;
using Xamarin.Forms;

[assembly: Dependency(typeof(ExerciseServiceManager))]
namespace MakeMeMove.iOS
{
    public class ExerciseServiceManager : IServiceManager
    {
        public void StartNotificationService(ExerciseSchedule schedule)
        {
            throw new NotImplementedException();
        }

        public void StopNotificationService(ExerciseSchedule schedule)
        {
            throw new NotImplementedException();
        }
    }
}
