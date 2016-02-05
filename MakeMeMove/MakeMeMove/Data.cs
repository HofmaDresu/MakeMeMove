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

            if (!ExerciseSchedules.Any())
            {
                var defaultSchedule = ExerciseSchedule.CreateDefaultSchedule();
                _db.Insert(defaultSchedule);
            }
            
            _db.CreateTable<ExerciseBlock>();

            if (!ExerciseSchedules.Any() && !ExerciseBlocks.Any())
            {
                var defaultExercises = ExerciseBlock.CreateDefaultExercises();
                _db.InsertAll(defaultExercises);
            }
        }

        public ExerciseSchedule GetExerciseSchedule()
        {
            var schedule = ExerciseSchedules.First();
            return schedule;
        }
        
        public void SaveExerciseSchedule(ExerciseSchedule exerciseSchedule)
        {
            _db.Update(exerciseSchedule);
        }

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
    }
}
