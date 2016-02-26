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
        private SQLiteConnection _db;
        private TableQuery<ExerciseSchedule> ExerciseSchedules => _db.Table<ExerciseSchedule>();
        private TableQuery<ExerciseBlock> ExerciseBlocks => _db.Table<ExerciseBlock>();
        private TableQuery<ExerciseHistory> ExerciseHistories => _db.Table<ExerciseHistory>();

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

            _db.CreateTable<ExerciseSchedule>();

            var hasExerciseSchedule = ExerciseSchedules.Any();
            if (!hasExerciseSchedule)
            {
                var defaultSchedule = ExerciseSchedule.CreateDefaultSchedule();
                _db.Insert(defaultSchedule);
            }
            
            _db.CreateTable<ExerciseBlock>();

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
        }

#region Schedule
        public ExerciseSchedule GetExerciseSchedule()
        {
            var schedule = ExerciseSchedules.First();
            return schedule;
        }
        
        public void SaveExerciseSchedule(ExerciseSchedule exerciseSchedule)
        {
            _db.Update(exerciseSchedule);
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

        public ExerciseBlock GetNextEnabledExercise()
        {
            var exercises = GetExerciseBlocks();
            var enabledExercises = exercises.Where(e => e.Enabled).ToList();

            if (enabledExercises.Count == 0) return null;

            var index = new Random().Next(0, enabledExercises.Count);

            return enabledExercises[Min(index, exercises.Count - 1)];
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

#endregion
    }
}
