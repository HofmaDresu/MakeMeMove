using System;
using MakeMeMove.Model;

namespace MakeMeMove
{
    public static class TickUtility
    {

        public static DateTime GetNextRunTime(ExerciseSchedule schedule, DateTime? fromDate = null)
        {
            var fromDateValue = fromDate.GetValueOrDefault(DateTime.Now);
            if (fromDateValue.TimeOfDay > schedule.EndTime.TimeOfDay)
            {
                return GetStartOfNextDay(schedule, fromDateValue);
            }

            if (fromDateValue.TimeOfDay < schedule.StartTime.TimeOfDay)
            {
                return GetStartOfToday(schedule, fromDateValue);
            }

            switch (schedule.Period)
            {
#if DEBUG
                case SchedulePeriod.EveryFiveMinutes:
                    return GetNextXMinuteRun(schedule, fromDateValue, 5);
#endif
                case SchedulePeriod.HalfHourly:
                    return GetNextHalfHourlyRun(schedule, fromDateValue);
                case SchedulePeriod.Hourly:
                    return GetNextHourlyRun(schedule, fromDateValue);
                case SchedulePeriod.BiHourly:
                    return GetNextBiHourlyRun(schedule, fromDateValue);
                case SchedulePeriod.EveryFifteenMinutes:
                    return GetNextXMinuteRun(schedule, fromDateValue, 15);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static DateTime GetNextXMinuteRun(ExerciseSchedule schedule, DateTime fromDateValue, int minuteInterval)
        {
            var fromDatePreviousInterval = (fromDateValue.Minute/ minuteInterval) * minuteInterval;
            var fromDatePreviousIntervalValue = ZeroOutMinutesAndLower(fromDateValue).AddMinutes(fromDatePreviousInterval);
            return GetStartNextDayIfOverTodaysEnd(schedule, fromDatePreviousIntervalValue.AddMinutes(minuteInterval));
        }

        private static DateTime GetNextHalfHourlyRun(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            var targetDate = fromDateValue.Minute < 30 
                ? new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, fromDateValue.Hour, 30, 0) 
                : ZeroOutMinutesAndLower(fromDateValue).AddHours(1);

            return GetStartNextDayIfOverTodaysEnd(schedule, targetDate);
        }

        private static DateTime GetNextHourlyRun(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            return GetStartNextDayIfOverTodaysEnd(schedule, ZeroOutMinutesAndLower(fromDateValue.AddHours(1)));
        }

        private static DateTime GetNextBiHourlyRun(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            return GetStartNextDayIfOverTodaysEnd(schedule, ZeroOutMinutesAndLower(fromDateValue.AddHours(2)));
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
            return GetStartOfToday(schedule, fromDateValue).AddDays(1);
        }

        private static DateTime GetStartOfToday(ExerciseSchedule schedule, DateTime fromDateValue)
        {
            return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
        }

        private static DateTime ZeroOutMinutesAndLower(DateTime dateTime)
        {
            return dateTime.AddMinutes(-1 * dateTime.Minute).AddSeconds(-1 * dateTime.Second).AddMilliseconds(-1 * dateTime.Millisecond);
        }
    }
}
