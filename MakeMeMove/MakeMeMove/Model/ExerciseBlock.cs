using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
