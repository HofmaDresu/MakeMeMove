using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Droid.DeviceSpecificImplementations;
using MakeMeMove.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Environment = System.Environment;

[assembly: Dependency(typeof(ProgressPersistence))]
namespace MakeMeMove.Droid.DeviceSpecificImplementations
{
    public class ProgressPersistence : IProgressPersistence
    {
        public void SaveCompletedExercise(DateTime dateCompleted, string exerciseName, int quantity)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ProgressData);
            var savedData = new Dictionary<DateTime, Dictionary<string, int>>();
            if (File.Exists(filePath))
            {
                savedData = JsonConvert.DeserializeObject<Dictionary<DateTime, Dictionary<string, int>>>(File.ReadAllText(filePath));

                if (savedData.ContainsKey(dateCompleted))
                {
                    var todaysData = savedData[dateCompleted];
                    if (todaysData.ContainsKey(exerciseName))
                    {
                        todaysData[exerciseName] += quantity;
                    }
                    else
                    {
                        todaysData.Add(exerciseName, quantity);
                    }
                }
                else
                {
                    savedData.Add(dateCompleted, new Dictionary<string, int> { { exerciseName, quantity }});
                }
            }
            else
            {
                savedData = new Dictionary<DateTime, Dictionary<string, int>> {{dateCompleted, new Dictionary<string, int> { { exerciseName, quantity} } }};
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(savedData));
        }

        public Dictionary<string, int> GetCompletedForDay(DateTime requestedDate)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ProgressData);
            var savedData = new Dictionary<DateTime, Dictionary<string, int>>();
            if (File.Exists(filePath))
            {
                savedData = JsonConvert.DeserializeObject<Dictionary<DateTime, Dictionary<string, int>>>(File.ReadAllText(filePath));

                if (savedData.ContainsKey(requestedDate))
                {
                    return savedData[requestedDate];
                }
            }

            return new Dictionary<string, int>();
        }

        public void RemoveAllData()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, Constants.ProgressData);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}