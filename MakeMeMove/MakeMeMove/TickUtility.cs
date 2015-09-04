using System;
using MakeMeMove.Model;

namespace MakeMeMove
{
    public static class TickUtility
    {
        public static int GetPollPeriod(SchedulePeriod schedulePeriod)
        {
            switch (schedulePeriod)
            {
                case SchedulePeriod.HalfHourly:
                    return MinutesToMilliseconds(30);
                case SchedulePeriod.Hourly:
                    return MinutesToMilliseconds(60);
                case SchedulePeriod.BiHourly:
                    return MinutesToMilliseconds(120);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static DateTime GetNextRunTime(ExerciseSchedule schedule, DateTime? fromDate = null)
        {
            var fromDateValue = fromDate.GetValueOrDefault(DateTime.Now);
            if (fromDateValue.TimeOfDay > schedule.EndTime.TimeOfDay)
            {
                return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day + 1, schedule.StartTime.Hour, schedule.StartTime.Minute,schedule.StartTime.Second);
            }

            if (fromDateValue.TimeOfDay < schedule.StartTime.TimeOfDay)
            {
                return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            }

            switch (schedule.Period)
            {
                case SchedulePeriod.HalfHourly:
                    return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, (fromDateValue.Minute < 30) ? fromDateValue.Hour : fromDateValue.Hour + 1, (fromDateValue.Minute < 30) ? 30 : 0, 0);
                case SchedulePeriod.Hourly:
                    return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day, fromDateValue.Hour + 1, 0, 0);
                case SchedulePeriod.BiHourly:
                    return new DateTime(fromDateValue.Year, fromDateValue.Month, fromDateValue.Day,
                        FindFirstRunHourForBiHourly(schedule.StartTime.Hour, schedule.EndTime.Hour, fromDateValue), 0, 0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int FindFirstRunHourForBiHourly(int startHour, int endHour, DateTime now)
        {
            var firstRunHour = startHour;
            for (; firstRunHour <= endHour; firstRunHour += 2)
            {
                if (firstRunHour > now.Hour)
                {
                    return firstRunHour;
                }
            }

            return startHour;
        }

        private static int MinutesToMilliseconds(int minutes)
        {
            return minutes*SecondsToMilliseconds(60);
        }

        private static int SecondsToMilliseconds(int seconds)
        {
            return seconds*1000;
        }
    }
}
