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
        private readonly ExerciseSchedule _exerciseSchedule;

        public EditSchedule(ExerciseSchedule schedule)
        {
            _exerciseSchedule = schedule;
            ViewModel = new EditScheduleViewModel ();

            InitializeComponent();

            InitializePickers();

            PeriodPicker.SelectedIndex = (int)schedule.Period;

            var civilianModifiedStartHour = (schedule.StartTime.Hour > 11
                ? schedule.StartTime.Hour - 12
                : schedule.StartTime.Hour);

            StartHourPicker.SelectedIndex = civilianModifiedStartHour == 0 ? 12 : civilianModifiedStartHour - 1;
            StartMinutePicker.SelectedIndex = schedule.StartTime.Minute == 0 ? 0 : 1;
            StartMeridianPicker.SelectedIndex = schedule.StartTime.Hour < 12 ? 0 : 1;


            var civilianModifiedEndHour = (schedule.EndTime.Hour > 11
                ? schedule.EndTime.Hour - 12
                : schedule.EndTime.Hour);

            EndHourPicker.SelectedIndex = civilianModifiedEndHour == 0 ? 12 : civilianModifiedEndHour - 1;
            EndMinutePicker.SelectedIndex = schedule.EndTime.Minute == 0 ? 0 : 1;
            EndMeridianPicker.SelectedIndex = schedule.EndTime.Hour < 12 ? 0 : 1;
        }

        private void InitializePickers()
        {
            foreach (SchedulePeriod suit in Enum.GetValues(typeof (SchedulePeriod)))
            {
                PeriodPicker.Items.Add(suit.Humanize());
            }

            for (var i = 1; i <= 12; i++)
            {
                StartHourPicker.Items.Add(i.ToString());
                EndHourPicker.Items.Add(i.ToString());
            }

            StartMinutePicker.Items.Add("00");
            StartMinutePicker.Items.Add("30");
            EndMinutePicker.Items.Add("00");
            EndMinutePicker.Items.Add("30");

            StartMeridianPicker.Items.Add("AM");
            StartMeridianPicker.Items.Add("PM");
            EndMeridianPicker.Items.Add("AM");
            EndMeridianPicker.Items.Add("PM");
        }

        private void SaveData(object sender, EventArgs e)
        {
            _exerciseSchedule.Period = (SchedulePeriod)PeriodPicker.SelectedIndex;
            _exerciseSchedule.StartTime = new DateTime(1, 1, 1, StartHourPicker.SelectedIndex + (12 * StartMeridianPicker.SelectedIndex), StartMinutePicker.SelectedIndex * 30, 0);
            _exerciseSchedule.EndTime = new DateTime(1, 1, 1, EndHourPicker.SelectedIndex + (12 * EndMeridianPicker.SelectedIndex), EndMinutePicker.SelectedIndex * 30, 0);



            Navigation.PopAsync(true);
        }

        private void CancelChanges(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
        }
    }
}
