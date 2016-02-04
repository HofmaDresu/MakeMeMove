using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

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

    public class ExerciseBlock
    {
        public Guid Id { get; set; }

        public string IdString => Id.ToString();

        public string Name { get; set; }
        public string CombinedName => string.IsNullOrWhiteSpace(Name) ? Type.Humanize() : Name;
        public PreBuiltExersises Type;
        public int Quantity { get; set; }
        public bool Enabled { get; set; }
    }
}
