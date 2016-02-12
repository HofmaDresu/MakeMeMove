using System;
using MakeMeMove;
using MakeMeMove.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TickUtilityTests
    {
        private readonly ExerciseSchedule _schedule = new ExerciseSchedule
        {
            StartTime = new DateTime(1, 1, 1, 8, 0, 0),
            EndTime = new DateTime(1, 1, 1, 17, 30, 0)
        };
        //DO NOT CHANGE, LOGIC DEPENDS ON THIS BEING AT THE VERY BEGINNING OF THE DAY (plus this tests both year and month rollover)
        private readonly DateTime _startTime = new DateTime(2015, 12, 31, 0, 0, 0);

        [TestMethod]
        public void TestHalfHourlySchedule()
        {
            _schedule.Period = SchedulePeriod.HalfHourly;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
            {
                var nextRunTime = TickUtility.GetNextRunTime(_schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0 || nextRunTime.Minute == 30, $"Minutes generated for {testTime} must equal 0 or 30. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < _schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(), nextRunTime);
                }
                else if (testTime.TimeOfDay >= _schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(), nextRunTime);
                }
                else
                {
                    if (testTime.Minute < 30)
                    {
                        Assert.IsTrue(nextRunTime.Minute == 30, $"Minutes must be set to 30 for {testTime}. Current value is {nextRunTime.Minute}");
                    }
                    else
                    {
                        Assert.IsTrue(nextRunTime.Minute == 0, $"Minutes must be set to 0 for {testTime}. Current value is {nextRunTime.Minute}");
                    }

                }
            }
        }

        [TestMethod]
        public void TestHourlySchedule()
        {
            _schedule.Period = SchedulePeriod.Hourly;
            DateTime? previousRunTime = null;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
            {
                var nextRunTime = TickUtility.GetNextRunTime(_schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0, $"Minutes generated for {testTime} must equal 0. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < _schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(), nextRunTime);
                }
                else if (testTime.TimeOfDay >= _schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(), nextRunTime);
                }
                else if (previousRunTime.HasValue)
                {
                    if (testTime.Hour == 23)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime() == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (testTime.Hour == previousRunTime.Value.AddHours(-1).Hour)
                    {
                        Assert.IsTrue(previousRunTime.Value == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (previousRunTime.Value.Hour == _schedule.EndTime.Hour)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime()== nextRunTime, $"Start Time: {GetTomorrowsStartTime()}, Next Run Time {nextRunTime}");
                    }
                    else
                    {
                        Assert.IsTrue(previousRunTime.Value.AddHours(1) == nextRunTime
                            || GetTomorrowsStartTime() == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                }

                previousRunTime = nextRunTime;
            }
        }

        [TestMethod]
        public void TestBiHourlySchedule()
        {
            _schedule.Period = SchedulePeriod.BiHourly;
            _schedule.EndTime = new DateTime(1, 1, 1, 20, 0, 0);
            DateTime? previousRunTime = null;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); )
            {
                var nextRunTime = TickUtility.GetNextRunTime(_schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0, $"Minutes generated for {testTime} must equal 0. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < _schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(), nextRunTime);
                }
                else if (testTime.TimeOfDay >= _schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(), nextRunTime);
                }
                else if (previousRunTime.HasValue)
                {
                    Assert.IsTrue(previousRunTime.Value == nextRunTime 
                        || previousRunTime.Value.AddHours(2) == nextRunTime
                        || nextRunTime == GetTomorrowsStartTime(), $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                }

                previousRunTime = nextRunTime;
                testTime = nextRunTime;
            }
        }

        private DateTime GetTomorrowsStartTime()
        {
            return GetTodaysStartTime().AddDays(1);
        }

        private DateTime GetTodaysStartTime()
        {
            return _startTime.AddHours(_schedule.StartTime.Hour).AddMinutes(_schedule.StartTime.Minute);
        }
    }
}
