using System;
using MakeMeMove;
using MakeMeMove.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TickUtilityTests
    {
        //DO NOT CHANGE, LOGIC DEPENDS ON THIS BEING AT THE VERY BEGINNING OF THE DAY (plus this tests both year and month rollover)
        private readonly DateTime _startTime = new DateTime(2015, 12, 31, 0, 0, 0);

        [TestMethod]
        public void TestHalfHourlySchedule()
        {
            var  schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.HalfHourly
            };

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
            {
                var nextRunTime = TickUtility.GetNextRunTime(schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0 || nextRunTime.Minute == 30, $"Minutes generated for {testTime} must equal 0 or 30. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(schedule), nextRunTime);
                }
                else if (testTime.TimeOfDay >= schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(schedule), nextRunTime);
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
            var schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.Hourly
            };
            DateTime? previousRunTime = null;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
            {
                var nextRunTime = TickUtility.GetNextRunTime(schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0, $"Minutes generated for {testTime} must equal 0. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(schedule), nextRunTime);
                }
                else if (testTime.TimeOfDay >= schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(schedule), nextRunTime);
                }
                else if (previousRunTime.HasValue)
                {
                    if (testTime.Hour == 23)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime(schedule) == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (testTime.Hour == previousRunTime.Value.AddHours(-1).Hour)
                    {
                        Assert.IsTrue(previousRunTime.Value == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (previousRunTime.Value.Hour == schedule.EndTime.Hour)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime(schedule) == nextRunTime, $"Start Time: {GetTomorrowsStartTime(schedule)}, Next Run Time {nextRunTime}");
                    }
                    else
                    {
                        Assert.IsTrue(previousRunTime.Value.AddHours(1) == nextRunTime
                            || GetTomorrowsStartTime(schedule) == nextRunTime, $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                }

                previousRunTime = nextRunTime;
            }
        }

        [TestMethod]
        public void TestBiHourlySchedule()
        {
            var schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.BiHourly
            };
            RunBiHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestBiHourlySchedule_EndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.BiHourly
            };
            RunBiHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestBiHourlySchedule_BeginningOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.BiHourly
            };
            RunBiHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestBiHourlySchedule_BeginningAndEndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.BiHourly
            };
            RunBiHourlyTestLoop(schedule);
        }

        private void RunBiHourlyTestLoop(ExerciseSchedule schedule)
        {
            DateTime? previousRunTime = null;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1);)
            {
                var nextRunTime = TickUtility.GetNextRunTime(schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0,
                    $"Minutes generated for {testTime} must equal 0. Current value is {nextRunTime.Minute}");

                if (testTime.TimeOfDay < schedule.StartTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTodaysStartTime(schedule), nextRunTime);
                }
                else if (testTime.TimeOfDay >= schedule.EndTime.TimeOfDay)
                {
                    Assert.AreEqual(GetTomorrowsStartTime(schedule), nextRunTime);
                }
                else if (previousRunTime.HasValue)
                {
                    Assert.IsTrue(previousRunTime.Value == nextRunTime
                                  || previousRunTime.Value.AddHours(2) == nextRunTime
                                  || nextRunTime == GetTomorrowsStartTime(schedule),
                        $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                }

                previousRunTime = nextRunTime;
                testTime = nextRunTime;
            }
        }

        private DateTime GetTomorrowsStartTime(ExerciseSchedule schedule)
        {
            return GetTodaysStartTime(schedule).AddDays(1);
        }

        private DateTime GetTodaysStartTime(ExerciseSchedule schedule)
        {
            return _startTime.AddHours(schedule.StartTime.Hour).AddMinutes(schedule.StartTime.Minute);
        }
    }
}
