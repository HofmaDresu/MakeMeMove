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
                case SchedulePeriod.HalfHourly:
                    return GetNextHalfHourlyRun(fromDateValue);
                case SchedulePeriod.Hourly:
                    return GetNextHourlyRun(schedule, fromDateValue);
                case SchedulePeriod.BiHourly:
                    return GetNextBiHourlyRun(schedule, fromDateValue);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static DateTime GetNextHalfHourlyRun(DateTime fromDateValue)
        {
            return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, (fromDateValue.Minute < 30) ? fromDateValue.Hour : fromDateValue.Hour + 1, (fromDateValue.Minute < 30) ? 30 : 0, 0);
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
            return target.TimeOfDay <= schedule.EndTime.TimeOfDay ? target : GetStartOfNextDay(schedule, target);
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
            return ZeroOutSecondsAndLower(dateTime).AddMinutes(-1*dateTime.Minute);
        }

        private static DateTime ZeroOutSecondsAndLower(DateTime dateTime)
        {
            return dateTime.AddSeconds(-1*dateTime.Second).AddMilliseconds(-1*dateTime.Millisecond);
        }
    }
}
