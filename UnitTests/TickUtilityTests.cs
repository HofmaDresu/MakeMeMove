using System;
using System.Collections.Generic;
using System.Linq;
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
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.HalfHourly
            };

            RunHalfHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHalfHourlySchedule_EndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 23, 30, 0),
                Period = SchedulePeriod.HalfHourly
            };

            RunHalfHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHalfHourlySchedule_BeginningOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.HalfHourly
            };

            RunHalfHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHalfHourlySchedule_BeginningAndEndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 23, 30, 0),
                Period = SchedulePeriod.HalfHourly
            };

            RunHalfHourlyTestLoop(schedule);
        }

        private void RunHalfHourlyTestLoop(ExerciseSchedule schedule)
        {
            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
            {
                var nextRunTime = TickUtility.GetNextRunTime(schedule, testTime);

                Assert.IsTrue(nextRunTime.Minute == 0 || nextRunTime.Minute == 30,
                    $"Minutes generated for {testTime} must equal 0 or 30. Current value is {nextRunTime.Minute}");

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
                        Assert.IsTrue(nextRunTime.Minute == 30,
                            $"Minutes must be set to 30 for {testTime}. Current value is {nextRunTime.Minute}");
                    }
                    else
                    {
                        Assert.IsTrue(nextRunTime.Minute == 0,
                            $"Minutes must be set to 0 for {testTime}. Current value is {nextRunTime.Minute}");
                    }
                }
            }
        }

        [TestMethod]
        public void TestHourlySchedule()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.Hourly
            };
            RunHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHourlySchedule_EndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 0, 0),
                EndTime = new DateTime(1, 1, 1, 23, 30, 0),
                Period = SchedulePeriod.Hourly
            };
            RunHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHourlySchedule_BeginningOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 17, 30, 0),
                Period = SchedulePeriod.Hourly
            };
            RunHourlyTestLoop(schedule);
        }

        [TestMethod]
        public void TestHourlySchedule_BeginningAndEndOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 23, 30, 0),
                Period = SchedulePeriod.Hourly
            };
            RunHourlyTestLoop(schedule);
        }

        private void RunHourlyTestLoop(ExerciseSchedule schedule)
        {
            DateTime? previousRunTime = null;

            for (var testTime = _startTime; testTime <= _startTime.AddDays(1).AddMinutes(-1); testTime = testTime.AddMinutes(1))
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
                    if (testTime.Hour == 23)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime(schedule) == nextRunTime,
                            $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (testTime.Hour == previousRunTime.Value.AddHours(-1).Hour)
                    {
                        Assert.IsTrue(previousRunTime.Value == nextRunTime,
                            $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
                    }
                    else if (previousRunTime.Value.Hour == schedule.EndTime.Hour)
                    {
                        Assert.IsTrue(GetTomorrowsStartTime(schedule) == nextRunTime,
                            $"Start Time: {GetTomorrowsStartTime(schedule)}, Next Run Time {nextRunTime}");
                    }
                    else
                    {
                        Assert.IsTrue(previousRunTime.Value.AddHours(1) == nextRunTime
                                      || GetTomorrowsStartTime(schedule) == nextRunTime,
                            $"Previous Run Time: {previousRunTime.Value}, Next Run Time {nextRunTime}");
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
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
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
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
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
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
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
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
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

        [TestMethod]
        public void TestFifteenMinuteSchedule_Start0End15()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 15, 0, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(15, nextRunTime.Hour);
            Assert.AreEqual(15, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_Start15End30()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 15, 15, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(15, nextRunTime.Hour);
            Assert.AreEqual(30, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_Start30End45()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 15, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(15, nextRunTime.Hour);
            Assert.AreEqual(45, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_Start45End0()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 0, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 15, 45, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(16, nextRunTime.Hour);
            Assert.AreEqual(0, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_HitLastTimeOfDay()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 0, 0, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 22, 15, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(22, nextRunTime.Hour);
            Assert.AreEqual(30, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_LoopToTomorrow()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 22, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(8, nextRunTime.Hour);
            Assert.AreEqual(30, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestFifteenMinuteSchedule_AdvanceToStart()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.EveryDay,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };

            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 7, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(8, nextRunTime.Hour);
            Assert.AreEqual(30, nextRunTime.Minute);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnMonday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekdaysOnly,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Except(new List<DayOfWeek> {DayOfWeek.Saturday, DayOfWeek.Sunday}).ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(_startTime.Year, _startTime.Month, _startTime.Day, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(thisRunTime.AddMinutes(15), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnSaturday_AdvanceToMonday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekdaysOnly,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Except(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }).ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 6, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(schedule.StartTime.AddDays(7), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnSunday_AdvanceToMonday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekdaysOnly,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Except(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }).ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 7, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(schedule.StartTime.AddDays(7), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_LastOnFriday_AdvanceToMonday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekdaysOnly,
                ScheduledDays = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Except(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday }).ToList(),
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 5, 22, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(schedule.StartTime.AddDays(7), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnSaturday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekendsOnly,
                ScheduledDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 6, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(thisRunTime.AddMinutes(15), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnSunday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekendsOnly,
                ScheduledDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 7, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(thisRunTime.AddMinutes(15), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_RunOnMonday_AdvanceToSaturday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekendsOnly,
                ScheduledDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 1, 9, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(schedule.StartTime.AddDays(5), nextRunTime);
        }

        [TestMethod]
        public void TestWeekdayOnlySchedule_LastOnSunday_AdvanceToSaturday()
        {
            var schedule = new ExerciseSchedule
            {
                Type = ScheduleType.WeekendsOnly,
                ScheduledDays = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                StartTime = new DateTime(1, 1, 1, 8, 30, 0),
                EndTime = new DateTime(1, 1, 1, 22, 30, 0),
                Period = SchedulePeriod.EveryFifteenMinutes
            };
            var thisRunTime = new DateTime(1, 1, 7, 22, 30, 0);
            var nextRunTime = TickUtility.GetNextRunTime(schedule, thisRunTime);

            Assert.AreEqual(schedule.StartTime.AddDays(12), nextRunTime);
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
