using System.Collections.Generic;
using Humanizer;
using SQLite;

namespace MakeMeMove.Model
{
    public enum PreBuiltExersises
    {
        PushUps,
        SitUps,
        JumpingJacks,
        Squats,
        CalfRaises,
        Lunges,
        Custom
    }

    [Table("ExerciseBlocks")]
    public class ExerciseBlock
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        [Ignore]
        public string CombinedName => string.IsNullOrWhiteSpace(Name) ? Type.Humanize() : Name;


        
        public PreBuiltExersises Type { get; set; }
        public int Quantity { get; set; }
        public bool Enabled { get; set; }

        public static List<ExerciseBlock> CreateDefaultExercises()
        {
            return new List<ExerciseBlock>
            {
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.PushUps.Humanize(),
                    Type = PreBuiltExersises.PushUps,
                    Quantity = 5,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.SitUps.Humanize(),
                    Type = PreBuiltExersises.SitUps,
                    Quantity = 5,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.JumpingJacks.Humanize(),
                    Type = PreBuiltExersises.JumpingJacks,
                    Quantity = 5,
                    Enabled = true
                },
                new ExerciseBlock
                {
                    Name = PreBuiltExersises.CalfRaises.Humanize(),
                    Type = PreBuiltExersises.CalfRaises,
                    Quantity = 10,
                    Enabled = true
                }
            };
        }
    }

    [Table("MostRecentExercise")]
    public class MostRecentExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        [Ignore]
        public string CombinedName => string.IsNullOrWhiteSpace(Name) ? Type.Humanize() : Name;
        public PreBuiltExersises Type { get; set; }
        public int Quantity { get; set; }

        public static MostRecentExercise FromBlock(ExerciseBlock block)
        {
            return new MostRecentExercise
            {
                Name = block.Name,
                Type = block.Type,
                Quantity = block.Quantity
            };
        }
    }
}
