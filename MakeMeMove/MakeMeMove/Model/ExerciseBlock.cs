using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Guid? Id { get; set; }

        public string IdString => Id.ToString();

        public string Name { get; set; }
        public PreBuiltExersises Type;
        public int Quantity { get; set; }
    }
}
