using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.DeviceSpecificInterfaces;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class ViewProgress : ContentPage
    {
        private readonly IProgressPersistence _progressPersistence;

        public ViewProgress()
        {
            _progressPersistence = DependencyService.Get<IProgressPersistence>();
            

            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StatTitle.Text = "Statistics for " + DateTime.Now.ToString("M/d/yyyy");
            var exerciseProgressItems = _progressPersistence.GetCompletedForDay(DateTime.Now.Date)
                .Select(p => new ExerciseProgressItem { Name = p.Key, Quantity = p.Value }).ToList();
            ProgressList.ItemsSource =
                exerciseProgressItems;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e == null) return;
            ((ListView)sender).SelectedItem = null; // de-select the row
        }

        public class ExerciseProgressItem
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
        }
    }
}
