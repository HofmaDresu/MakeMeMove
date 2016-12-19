using System;
using System.Linq;
using System.Collections.Generic;
using Humanizer;
using SQLite;

namespace MakeMeMove.Model
{
    public enum SchedulePeriod
    {
        HalfHourly = 0,
        Hourly = 1,
        BiHourly = 2,
        EveryFifteenMinutes = 3,
#if DEBUG
        //For Debug use only, this is not well tested and may break under real conditions
        EveryFiveMinutes = 4
#endif
    }

    public enum ScheduleType
    {
        EveryDay = 0,
        WeekendsOnly = 1,
        WeekdaysOnly = 2,
        Custom = 3
    }

    [Table("ExerciseSchedules")]
    public class ExerciseSchedule
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public SchedulePeriod Period { get; set; }
        public ScheduleType Type { get; set; }
        public string CustomDays { get; set; }

        [Ignore]
        public List<DayOfWeek> ScheduledDays
        {
            get
            {
                switch (Type)
                {
                    case ScheduleType.EveryDay:
                        return Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
                    case ScheduleType.WeekendsOnly:
                        return new List<DayOfWeek> {DayOfWeek.Saturday, DayOfWeek.Sunday};
                    case ScheduleType.WeekdaysOnly:
                        return Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Except(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }).ToList();
                    case ScheduleType.Custom:
                        return string.IsNullOrWhiteSpace(CustomDays)
                            ? new List<DayOfWeek>()
                            : CustomDays.Split(Constants.DatabaseListSeparator).Select(GetDayOfWeekFromString)
                                        .Where(d => d.HasValue).Select(d => d.Value).ToList();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private DayOfWeek? GetDayOfWeekFromString(string d)
        {
            int day;
            if (int.TryParse(d, out day))
            {
                try
                {
                    return (DayOfWeek?) day;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        [Ignore]
        public string PeriodDisplayString => Period.Humanize();

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        public static ExerciseSchedule CreateDefaultSchedule()
        {
            return new ExerciseSchedule
            {
#if DEBUG
                Period = SchedulePeriod.EveryFiveMinutes,
#else
                Period = SchedulePeriod.Hourly,
#endif
                StartTime = new DateTime(1, 1, 1, 8, 0, 0), EndTime = new DateTime(1, 1, 1, 17, 00, 0)
            };
        }
    }
}
