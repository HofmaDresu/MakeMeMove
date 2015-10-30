using System;
using Humanizer;
using MakeMeMove.DeviceSpecificInterfaces;
using MakeMeMove.Model;
using MakeMeMove.ViewModel;
using Xamarin.Forms;

namespace MakeMeMove.View
{
    public partial class EditSchedule : ContentPage
    {
        public EditScheduleViewModel ViewModel;
        private readonly ExerciseSchedule _exerciseSchedule;
        private readonly ISchedulePersistence _schedulePersistence;
        private readonly IServiceManager _notificationServiceManager;
        private readonly IUserNotification _userNotification;
        private bool _buttonsAreEnabled;

        public EditSchedule()
        {
            _schedulePersistence = DependencyService.Get<ISchedulePersistence>();
            _notificationServiceManager = DependencyService.Get<IServiceManager>();
            _userNotification = DependencyService.Get<IUserNotification>();

            _exerciseSchedule = !_schedulePersistence.HasExerciseSchedule() ? ExerciseSchedule.CreateDefaultSchedule() : _schedulePersistence.LoadExerciseSchedule();

            ViewModel = new EditScheduleViewModel ();

            InitializeComponent();

            InitializePickers();

            PeriodPicker.SelectedIndex = (int)_exerciseSchedule.Period;

            var civilianModifiedStartHour = (_exerciseSchedule.StartTime.Hour > 11
                ? _exerciseSchedule.StartTime.Hour - 12
                : _exerciseSchedule.StartTime.Hour);

            StartHourPicker.SelectedIndex = civilianModifiedStartHour == 0 ? 12 : civilianModifiedStartHour - 1;
            StartMinutePicker.SelectedIndex = _exerciseSchedule.StartTime.Minute == 0 ? 0 : 1;
            StartMeridianPicker.SelectedIndex = _exerciseSchedule.StartTime.Hour < 12 ? 0 : 1;


            var civilianModifiedEndHour = (_exerciseSchedule.EndTime.Hour > 11
                ? _exerciseSchedule.EndTime.Hour - 12
                : _exerciseSchedule.EndTime.Hour);
            
            EndHourPicker.SelectedIndex = civilianModifiedEndHour == 0 ? 12 : civilianModifiedEndHour - 1;
            EndMinutePicker.SelectedIndex = _exerciseSchedule.EndTime.Minute == 0 ? 0 : 1;
            EndMeridianPicker.SelectedIndex = _exerciseSchedule.EndTime.Hour < 12 ? 0 : 1;


            _buttonsAreEnabled = true;
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
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            _exerciseSchedule.Period = (SchedulePeriod)PeriodPicker.SelectedIndex;

            var startHour = StartHourPicker.SelectedIndex == 11 ? 0 : StartHourPicker.SelectedIndex + 1;
            var startTime = new DateTime(1, 1, 1, startHour + (12 * StartMeridianPicker.SelectedIndex), StartMinutePicker.SelectedIndex * 30, 0);

            var endHour = EndHourPicker.SelectedIndex == 11 ? 0 : EndHourPicker.SelectedIndex + 1;
            var endTime = new DateTime(1, 1, 1, endHour + (12 * EndMeridianPicker.SelectedIndex), EndMinutePicker.SelectedIndex * 30, 0);
            
            if (startTime >= endTime)
            {
                _userNotification.ShowValidationErrorPopUp("Please make sure your start time is before your end time");
                EnableButtons();
                return;
            }

            _exerciseSchedule.StartTime = startTime;
            _exerciseSchedule.EndTime = endTime;

            _schedulePersistence.SaveExerciseSchedule(_exerciseSchedule);
            _notificationServiceManager.RestartNotificationServiceIfNeeded(_exerciseSchedule);

            Navigation.PopAsync(true);
        }

        private void CancelChanges(object sender, EventArgs e)
        {
            if (!ButtonsAreEnabled()) return;
            DisableButtons();
            Navigation.PopAsync(true);
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
    }
}
