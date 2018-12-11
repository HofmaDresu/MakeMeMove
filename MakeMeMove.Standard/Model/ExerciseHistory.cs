using System;
using SQLite;

namespace MakeMeMove.Model
{

    [Table("ExerciseHistories")]
    public class ExerciseHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime RecordedDate { get; set; }
        public string ExerciseName { get; set; }
        public int QuantityCompleted { get; set; }
        public int QuantityNotified { get; set; }

    }
}
