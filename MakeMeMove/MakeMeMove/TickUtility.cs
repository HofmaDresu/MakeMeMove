using System;
using MakeMeMove.Model;

namespace MakeMeMove
{
    public static class TickUtility
    {
        public static DateTime GetNextRunTime(ExerciseSchedule schedule)
        {
            throw new NotImplementedException();
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
                    return GetNextHalfHourlyRun(schedule, fromDate);
                case SchedulePeriod.Hourly:
                    return GetNextHourlyRun(schedule, fromDate);
                case SchedulePeriod.BiHourly:
                    return GetNextBiHourlyRun(schedule, fromDate);
                case SchedulePeriod.EveryFifteenMinutes:
                    return GetNextXMinuteRun(schedule, fromDate, 15);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static DateTime GetCalculationDate(ExerciseSchedule schedule, DateTime? fromDate = null)
        {
            var now = DateTime.Now;
            if (fromDate.HasValue) return new DateTime(fromDate.Value.Year, fromDate.Value.Month, fromDate.Value.Day, fromDate.Value.Hour, fromDate.Value.Minute, 0);

            var calcDate = new DateTime(now.Year, now.Month, now.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, 0);
            return calcDate;
        }

        private static DateTime GetNextXMinuteRun(ExerciseSchedule schedule, DateTime fromDateValue, int minuteInterval)
        {
                
            return GetStartNextDayIfOverTodaysEnd(schedule, fromDateValue.AddMinutes(minuteInterval));
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

        private static DateTime ZeroOutMinutesAndLower(DateTime dateTime)
        {
            return ZeroOutSecondsAndLower(dateTime).AddMinutes(-1 * dateTime.Minute);
        }

        private static DateTime ZeroOutSecondsAndLower(DateTime dateTime)
        {
            return dateTime.AddSeconds(-1 * dateTime.Second).AddMilliseconds(-1 * dateTime.Millisecond);
        }
    }
}
