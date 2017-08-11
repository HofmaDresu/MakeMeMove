using System;
using System.Collections.Generic;
using System.Linq;
using MakeMeMove.Model;
using SQLite;
using static System.Math;

namespace MakeMeMove
{
    public class Data
    {
        private const int RatingCycleSize = 10;

        private SQLiteConnection _db;
        private TableQuery<ExerciseSchedule> ExerciseSchedules => _db.Table<ExerciseSchedule>();
        private TableQuery<ExerciseBlock> ExerciseBlocks => _db.Table<ExerciseBlock>();
        private TableQuery<MostRecentExercise> MostRecentExercises => _db.Table<MostRecentExercise>();
        private TableQuery<ExerciseHistory> ExerciseHistories => _db.Table<ExerciseHistory>();
        private TableQuery<FudistUser> FudistUsers => _db.Table<FudistUser>();
        private TableQuery<SystemStatus> SystemStatus => _db.Table<SystemStatus>();

        public int RatingCheckTimesOpened { get; private set; }

        private static readonly Lazy<Data> LazyData = new Lazy<Data>();

        public static Data GetInstance(SQLiteConnection conn)
        {
            var instance = LazyData.Value;
            instance.Initalize(conn);
            return instance;
        }

        private void Initalize(SQLiteConnection conn)
        {
            _db = conn;

            _db.CreateTable<SystemStatus>();
            
            if (!SystemStatus.Any())
            {
                _db.Insert(new SystemStatus {IsFirstRun = true});
            }
            _db.CreateTable<ExerciseSchedule>();

            var hasExerciseSchedule = ExerciseSchedules.Any();
            if (!hasExerciseSchedule)
            {
                var defaultSchedule = ExerciseSchedule.CreateDefaultSchedule();
                _db.Insert(defaultSchedule);
            }
            
            _db.CreateTable<ExerciseBlock>();
            _db.CreateTable<MostRecentExercise>();

            if (!hasExerciseSchedule && !ExerciseBlocks.Any())
            {
                var defaultExercises = ExerciseBlock.CreateDefaultExercises();
                _db.InsertAll(defaultExercises);
            }

            _db.CreateTable<ExerciseHistory>();

#if DEBUG
            var now = DateTime.Now.Date;
            if (!ExerciseHistories.Any(eh => eh.RecordedDate < now))
            {
                _db.InsertAll(new List<ExerciseHistory>
                {
                    new ExerciseHistory
                    {
                        QuantityNotified = 10,
                        ExerciseName = "test",
                        RecordedDate = DateTime.Now.AddDays(-1),
                        QuantityCompleted = 10
                    },
                    new ExerciseHistory
                    {
                        QuantityNotified = 10,
                        ExerciseName = "test",
                        RecordedDate = DateTime.Now.AddDays(-2),
                        QuantityCompleted = 10
                    },
                    new ExerciseHistory
                    {
                        QuantityNotified = 10,
                        ExerciseName = "test",
                        RecordedDate = DateTime.Now.AddDays(-3),
                        QuantityCompleted = 10
                    },
                });
            }
#endif


            _db.CreateTable<FudistUser>();
        }

#region Schedule
        public ExerciseSchedule GetExerciseSchedule()
        {
            var schedule = ExerciseSchedules.First();
            schedule.ScheduledDays = GetScheduleDays(schedule);
            return schedule;
        }
        
        public void SaveExerciseSchedule(ExerciseSchedule exerciseSchedule)
        {
            if (exerciseSchedule.Type == ScheduleType.Custom)
            {
                exerciseSchedule.CustomDays = string.Join(Constants.DatabaseListSeparator.ToString(), exerciseSchedule.ScheduledDays);
            }
            else
            {
                exerciseSchedule.CustomDays = string.Empty;
            }

            _db.Update(exerciseSchedule);
        }

        private List<DayOfWeek> GetScheduleDays(ExerciseSchedule schedule)
        {
            switch (schedule.Type)
            {
                case ScheduleType.EveryDay:
                    return Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
                case ScheduleType.WeekendsOnly:
                    return new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday };
                case ScheduleType.WeekdaysOnly:
                    return
                        Enum.GetValues(typeof(DayOfWeek))
                            .Cast<DayOfWeek>()
                            .Except(new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday })
                            .ToList();
                case ScheduleType.Custom:
                    return string.IsNullOrWhiteSpace(schedule.CustomDays)
                        ? new List<DayOfWeek>()
                        : schedule.CustomDays.Split(Constants.DatabaseListSeparator).Select(GetDayOfWeekFromString)
                            .Where(d => d.HasValue).Select(d => d.Value).ToList();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private DayOfWeek? GetDayOfWeekFromString(string d)
        {
            int day;
            if (int.TryParse(d, out day))
            {
                try
                {
                    return (DayOfWeek?)day;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        #endregion

        #region ExerciseBlocks
        public List<ExerciseBlock> GetExerciseBlocks()
        {
            return ExerciseBlocks.ToList();
        } 

        public void DeleteExerciseBlock(int id)
        {
            _db.Delete<ExerciseBlock>(id);
        }

        public void InsertExerciseBlock(ExerciseBlock newExerciseBlock)
        {
            _db.Insert(newExerciseBlock);
        }

        public void UpdateExerciseBlock(ExerciseBlock blockToUpdate)
        {
            _db.Update(blockToUpdate);
        }

        public ExerciseBlock GetExerciseById(int id)
        {
            return ExerciseBlocks.SingleOrDefault(e => e.Id == id);
        }

        public ExerciseBlock GetNextEnabledExercise(Random random = null, bool overrideDifferent = false)
        {
            ExerciseBlock nextExercise;

            if (MostRecentExercises.Any() && !overrideDifferent)
            {
                nextExercise = GetNextDifferentEnabledExercise(MostRecentExercises.First(), random);
            }
            else
            {
                var exercises = GetExerciseBlocks();
                var enabledExercises = exercises.Where(e => e.Enabled).ToList();

                if (enabledExercises.Count == 0) return null;

                var index = (random ?? new Random()).Next(0, enabledExercises.Count);

                nextExercise = enabledExercises[Min(index, enabledExercises.Count - 1)];
            }
            MostRecentExercises.Delete(_ => true);
            if(nextExercise != null) _db.Insert(MostRecentExercise.FromBlock(nextExercise));
            return nextExercise;
        }

        private ExerciseBlock GetNextDifferentEnabledExercise(MostRecentExercise mostRecentExercise, Random random = null)
        {
            var exercises = GetExerciseBlocks();
            var enabledExercises = exercises.Where(e => e.Enabled);
            // Look at name and quantitiy instead of Id since users can create multiple blocks that are the same
            var differentEnabledExercises = enabledExercises.Where(e =>
                    !e.CombinedName.Equals(mostRecentExercise.CombinedName, StringComparison.CurrentCultureIgnoreCase)
                    || (e.CombinedName.Equals(mostRecentExercise.CombinedName, StringComparison.CurrentCultureIgnoreCase)
                            && e.Quantity != mostRecentExercise.Quantity))
                .ToList();

            // If there are no different exercises, see if there are any available at all
            if (differentEnabledExercises.Count == 0) return GetNextEnabledExercise(random, true);

            var index = (random ?? new Random()).Next(0, differentEnabledExercises.Count);

            return differentEnabledExercises[Min(index, differentEnabledExercises.Count - 1)];
        }
#endregion

#region ExerciseHistory

        public List<ExerciseHistory> GetExerciseHistoryForDay(DateTime date)
        {
            return ExerciseHistories.Where(eh => eh.RecordedDate == date && eh.QuantityNotified > 0).OrderBy(eh => eh.ExerciseName).ToList();
        }

        public List<ExerciseHistory> GetPreviousExerciseHistory(DateTime currentDate)
        {
            return GetExerciseHistoryForDay(currentDate.AddDays(-1));
        }

        public List<ExerciseHistory> GetNextExerciseHistory(DateTime currentDate)
        {
            return GetExerciseHistoryForDay(currentDate.AddDays(1));
        }

        public DateTime GetMinimumExerciseHistoryDate()
        {
            return ExerciseHistories.Where(eh => eh.QuantityNotified > 0).OrderBy(eh => eh.RecordedDate).FirstOrDefault()?.RecordedDate ?? DateTime.Now.Date;
        }

        public void MarkExerciseNotified(string exerciseName, int quantity)
        {
            var exerciseHistory = ExerciseHistories.SingleOrDefault(eh =>
            {
                var todaysDate = DateTime.Today.Date;
                return eh.RecordedDate == todaysDate && eh.ExerciseName == exerciseName;
            });

            if (exerciseHistory == null)
            {
                exerciseHistory = new ExerciseHistory
                {
                    ExerciseName = exerciseName,
                    QuantityNotified = quantity,
                    RecordedDate = DateTime.Today
                };

                _db.Insert(exerciseHistory);
            }
            else
            {
                exerciseHistory.QuantityNotified += quantity;
                _db.Update(exerciseHistory);
            }
        }

        public void MarkExerciseCompleted(string exerciseName, int quantity)
        {
            var exerciseHistory = ExerciseHistories.SingleOrDefault(eh =>
            {
                var todaysDate = DateTime.Today.Date;
                return eh.RecordedDate == todaysDate && eh.ExerciseName == exerciseName;
            });

            if (exerciseHistory == null)
            {
                exerciseHistory = new ExerciseHistory
                {
                    ExerciseName = exerciseName,
                    QuantityCompleted = quantity,
                    RecordedDate = DateTime.Today
                };

                _db.Insert(exerciseHistory);
            }
            else
            {
                exerciseHistory.QuantityCompleted += quantity;
                _db.Update(exerciseHistory);
            }
        }

        public void DeleteAllHistory()
        {
            _db.DeleteAll<ExerciseHistory>();
        }

        public List<ExerciseTotal> GetExerciseTotals()
        {
            var totals = new Dictionary<string, int>();
            ExerciseHistories.Aggregate(totals, (t, h) =>
            {
                var name = h.ExerciseName;
                var quantity = h.QuantityCompleted;
                if (t.ContainsKey(name))
                {
                    t[name] += quantity;
                }
                else
                {
                    t.Add(name, quantity);
                }
                return t;
            });

            return totals.Select(t => new ExerciseTotal { ExerciseName = t.Key, QuantityCompleted = t.Value }).Where(t => t.QuantityCompleted > 0).ToList();
        }

        #endregion

#region SystemStatus
        /// <summary>
        /// Is the service running on iOS. Should not be used on Android
        /// </summary>
        /// <returns></returns>
        public bool IsIosServiceRunning()
        {
            var status = SystemStatus.First();
            return status.IosServiceIsRunning;
        }

        /// <summary>
        /// Set the service status in iOS. Should not be used on Android
        /// </summary>
        /// <param name="isRunning"></param>
        public void SetIosServiceRunningStatus(bool isRunning)
        {
            var status = SystemStatus.First();
            status.IosServiceIsRunning = isRunning;
            _db.Update(status);
        }

        public bool IsFirstRun()
        {
            var status = SystemStatus.First();
            return status.IsFirstRun;
        }

        public void MarkFirstRun()
        {
            var status = SystemStatus.First();
            status.IsFirstRun = false;
            _db.Update(status);
        }

        public bool ShouldAskForRating()
        {
            var status = SystemStatus.First();
            return status.AskForRating_DB_ONLY.GetValueOrDefault(true) && status.RatingCheckTimesOpened >= RatingCycleSize;
        }

        public void IncrementRatingCycle()
        {
            var status = SystemStatus.First();
            status.RatingCheckTimesOpened = Min(RatingCycleSize, status.RatingCheckTimesOpened + 1);
            _db.Update(status);
        }

        public void ResetRatingCycle()
        {
            var status = SystemStatus.First();
            status.RatingCheckTimesOpened = 0;
            _db.Update(status);
        }

        public void PreventRatingCheck()
        {
            var status = SystemStatus.First();
            status.AskForRating_DB_ONLY = false;
            _db.Update(status);
        }

        #endregion
    }
}
