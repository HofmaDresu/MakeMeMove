using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using MakeMeMove.Model;
using MakeMeMove.ViewModel;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class EditSchedule : ContentPage
    {
        public EditScheduleViewModel ViewModel;
        private ExerciseSchedule _exerciseSchedule;

        public EditSchedule(ExerciseSchedule schedule)
        {
            _exerciseSchedule = schedule;
            ViewModel = new EditScheduleViewModel ();




            InitializeComponent();

            foreach (SchedulePeriod suit in Enum.GetValues(typeof(SchedulePeriod)))
            {
                PeriodPicker.Items.Add(suit.Humanize());
            }

            ViewModel.SchedulePeriodIndex = (int)schedule.Period;
            PeriodPicker.SelectedIndex = ViewModel.SchedulePeriodIndex;
        }

        private void PeriodChanged(object sender, EventArgs e)
        {
            ViewModel.SchedulePeriodIndex = ((Picker) sender).SelectedIndex;
        }
    }
}
