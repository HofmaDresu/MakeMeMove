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

        public static DateTime GetNextRunTime(ExerciseSchedule schedule)
        {
            var now = DateTime.Now;
            if (now.TimeOfDay > schedule.EndTime.TimeOfDay)
            {
                return new DateTime(now.Year, now.Month, now.Day + 1, schedule.EndTime.Hour, schedule.EndTime.Minute,schedule.EndTime.Second);
            }

            if (now.TimeOfDay < schedule.StartTime.TimeOfDay)
            {
                return new DateTime(now.Year, now.Month, now.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second);
            }

            switch (schedule.Period)
            {
                case SchedulePeriod.HalfHourly:
                    return new DateTime(now.Year, now.Month, now.Day, (now.Minute < 30) ? now.Hour : now.Hour + 1, (now.Minute < 30) ? 30 : 0, 0);
                case SchedulePeriod.Hourly:
                    return new DateTime(now.Year, now.Month, now.Day, now.Hour + 1, 0, 0);
                case SchedulePeriod.BiHourly:
                    return new DateTime(now.Year, now.Month, now.Day,
                        FindFirstRunHourForBiHourly(schedule.StartTime.Hour, schedule.EndTime.Hour, now), 0, 0);
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
