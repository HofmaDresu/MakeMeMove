using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMeMove.Model;
using SQLite;

namespace MakeMeMove
{
    public class Data
    {
        private SQLiteConnection _db;
        private TableQuery<ExerciseSchedule> ExerciseSchedules => _db.Table<ExerciseSchedule>();
        private TableQuery<ExerciseBlock> ExerciseBlocks => _db.Table<ExerciseBlock>();

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
#endregion
    }
}
