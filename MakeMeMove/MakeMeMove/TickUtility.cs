using System;
using MakeMeMove.Model;

namespace MakeMeMove
{
    public static class TickUtility
    {

        public static DateTime GetNextRunTime(ExerciseSchedule schedule)
        {
            var nowValue = DateTime.Now;
            var todaysStartTime = new DateTime(nowValue.Year, nowValue.Month, nowValue.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, 0);
            while (todaysStartTime < nowValue)
            {
                todaysStartTime = GetNextRunTime(schedule, todaysStartTime);
            }
            return todaysStartTime;

        }

        /// <summary>
        /// Test method. This is used to allow unit tests to mock 'now'
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="nowValue"></param>
        /// <returns></returns>
        public static DateTime MockNow_GetNextRunTime(ExerciseSchedule schedule, DateTime nowValue)
        {
            var todaysStartTime = new DateTime(nowValue.Year, nowValue.Month, nowValue.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, 0);
            while (todaysStartTime < nowValue)
            {
                todaysStartTime = GetNextRunTime(schedule, todaysStartTime);
            }
            return todaysStartTime;
        }


        public static DateTime GetNextRunTime(ExerciseSchedule schedule, DateTime fromDate)
        {
            if (!schedule.ScheduledDays.Contains(fromDate.DayOfWeek) || fromDate.TimeOfDay > schedule.EndTime.TimeOfDay)
            {
                return GetStartOfNextDay(schedule, fromDate);
            }

            if (fromDate.TimeOfDay < schedule.StartTime.TimeOfDay)
            {
                return GetStartOfToday(schedule, fromDate);
            }

            switch (schedule.Period)
            {
#if DEBUG
                case SchedulePeriod.EveryFiveMinutes:
                    return GetNextXMinuteRun(schedule, fromDate, 5);
#endif
                case SchedulePeriod.HalfHourly:
                    return GetNextXMinuteRun(schedule, fromDate, 30);
                case SchedulePeriod.Hourly:
                    return GetNextXMinuteRun(schedule, fromDate, 60);
                case SchedulePeriod.BiHourly:
                    return GetNextXMinuteRun(schedule, fromDate, 120);
                case SchedulePeriod.EveryFifteenMinutes:
                    return GetNextXMinuteRun(schedule, fromDate, 15);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static DateTime GetNextXMinuteRun(ExerciseSchedule schedule, DateTime fromDateValue, int minuteInterval)
        {                
            return GetStartNextDayIfOverTodaysEnd(schedule, fromDateValue.AddMinutes(minuteInterval));
        }

        private static DateTime GetStartNextDayIfOverTodaysEnd(ExerciseSchedule schedule, DateTime target)
        {
            if (target.TimeOfDay <= schedule.EndTime.TimeOfDay && target.TimeOfDay >= schedule.StartTime.TimeOfDay)
            {
                return target;
            }
            if (target.TimeOfDay > schedule.EndTime.TimeOfDay)
            {
                return GetStartOfNextDay(schedule, target);
            }
            return GetStartOfToday(schedule, target);
        }

        private static DateTime GetStartOfNextDay(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            var targetDay = GetStartOfToday(schedule, fromDateValue).AddDays(1);
            while (!schedule.ScheduledDays.Contains(targetDay.DayOfWeek))
            {
                targetDay = targetDay.AddDays(1);
            }
            return targetDay;
        }

        private static DateTime GetStartOfToday(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
        }
    }
}
