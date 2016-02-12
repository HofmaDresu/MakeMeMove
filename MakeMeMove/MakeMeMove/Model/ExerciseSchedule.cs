using System;
using Humanizer;
using SQLite;

namespace MakeMeMove.Model
{
    public enum SchedulePeriod
    {
        HalfHourly,
        Hourly,
        BiHourly
    }

    [Table("ExerciseSchedules")]
    public class ExerciseSchedule
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public SchedulePeriod Period { get; set; }
        [Ignore]
        public string PeriodDisplayString => Period.Humanize();
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        public static ExerciseSchedule CreateDefaultSchedule()
        {
            return new ExerciseSchedule
            {
                Period = SchedulePeriod.HalfHourly,
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 19, 0, 0)
            };
        }

    }
}
