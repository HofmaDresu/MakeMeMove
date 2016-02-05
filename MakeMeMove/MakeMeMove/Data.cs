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
        private readonly SQLiteConnection _db;
        private TableQuery<ExerciseSchedule> ExerciseSchedules => _db.Table<ExerciseSchedule>();
        private TableQuery<ExerciseBlock> ExerciseBlocks => _db.Table<ExerciseBlock>();

        public Data(SQLiteConnection conn)
        {
            _db = conn;
            _db.Execute("drop table Exerciseschedules; drop table exerciseblocks;");

            _db.CreateTable<ExerciseSchedule>();
            _db.CreateTable<ExerciseBlock>();

            if (!ExerciseSchedules.Any())
            {
                var defaultSchedule = ExerciseSchedule.CreateDefaultSchedule();

                _db.Insert(defaultSchedule);
                _db.InsertAll(defaultSchedule.Exercises);
            }
        }

        public ExerciseSchedule GetExerciseSchedule()
        {
            var schedule = ExerciseSchedules.First();
            schedule.Exercises = ExerciseBlocks.ToList();
            return schedule;
        }
        
        public void SaveExerciseSchedule(ExerciseSchedule exerciseSchedule)
        {
            _db.Update(exerciseSchedule);

            DeleteRemovedExercises(exerciseSchedule);

            UpdateChangedExercises(exerciseSchedule);

            return GetExerciseSchedule();
        }

        private void DeleteRemovedExercises(ExerciseSchedule exerciseSchedule)
        {
            var currentExerciseIds = exerciseSchedule.Exercises.Select(e => e.Id).ToList();

            var exerciseIdsToDelete = (from e in ExerciseBlocks
                where !currentExerciseIds.Contains(e.Id)
                select e).Select(e => e.Id).ToList();

            ExerciseBlocks.Delete(eb => exerciseIdsToDelete.Contains(eb.Id));
        }

        private void UpdateChangedExercises(ExerciseSchedule exerciseSchedule)
        {
            foreach (var VARIABLE in COLLECTION)
            {
                
            }
        }
    }
}
