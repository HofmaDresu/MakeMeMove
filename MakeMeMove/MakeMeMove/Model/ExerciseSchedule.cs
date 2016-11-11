using System;
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
#if DEBUG
                Period = SchedulePeriod.EveryFiveMinutes,

#else
                Period = SchedulePeriod.Hourly,
#endif
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 00, 0)
            };
        }

    }
}
