using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.Model;

namespace MakeMeMove.DeviceSpecificInterfaces
{
    public interface IProgressPersistence
    {
        void SaveCompletedExercise(DateTime dateCompleted, string exerciseName, int quantity);
        Dictionary<string, int> GetCompletedForDay(DateTime requestedDate);
        void RemoveAllData();
    }
}
