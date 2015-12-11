using System;
using System.IO;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using Newtonsoft.Json;
using Xamarin.Forms;

[assembly: Dependency(typeof(SchedulePersistence))]
namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class SchedulePersistence : ISchedulePersistence
    {
        public void SaveExerciseSchedule(ExerciseSchedule schedule)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ExerciseSchedule);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(schedule));
        }

        public ExerciseSchedule LoadExerciseSchedule()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ExerciseSchedule);
            return JsonConvert.DeserializeObject<ExerciseSchedule>(File.ReadAllText(filePath));
        }

        public bool HasExerciseSchedule()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ExerciseSchedule);

            return File.Exists(filePath) && !string.IsNullOrWhiteSpace(File.ReadAllText(filePath));
        }

        public void RemoveAllData()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ExerciseSchedule);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            filePath = Path.Combine(documentsPath, Constants.ServiceIsStartedKey);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            filePath = Path.Combine(documentsPath, Constants.SharedPreferencesKey);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}